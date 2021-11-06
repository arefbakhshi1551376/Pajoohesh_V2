using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Actions
{
    public class SubjectActions : Controller
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<User> _userManager;
        private IWebHostEnvironment env;

        public SubjectActions(IUnitOfWork unitOfWork, UserManager<User> userManager, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            this.env = env;
        }


        public async Task SubjectAddAction(SubjectAddVm subjectAddVm, User user)
        {
            var defaultBuilder = new DefaultBuilder(_unitOfWork);
            await defaultBuilder.BuildDefaultSubject(user);
            var subjectTitles = (await _unitOfWork.SubjectRepository.GetAllAsync()).Select(s => s.Title);
            if (!subjectTitles.Contains(subjectAddVm.Title.Trim()))
            {
                string imageFinalUrl = null;

                if (subjectAddVm.ImageUrl != null)
                {
                    var extImage = Path.GetExtension(subjectAddVm.ImageUrl.FileName);
                    imageFinalUrl = Builder.BuildNameForImages() + extImage;
                    var pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Subject", imageFinalUrl);

                    await using var fileStream = new FileStream(pathImage, FileMode.Create);
                    await subjectAddVm.ImageUrl.CopyToAsync(fileStream);
                }
                else
                {
                    imageFinalUrl = "eeeeeeeeeeeeeeee.jpg";
                }

                var currentSubject = new Subject()
                {
                    Creator = user,
                    State = subjectAddVm.State,
                    Title = subjectAddVm.Title.Trim(),
                    CreateDate = DateTime.UtcNow,
                    LastModifier = null,
                    LastModifyDate = null,
                    ImageUrl = imageFinalUrl
                };
                await _unitOfWork.SubjectRepository.AddAsync(currentSubject);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task SubjectEditAction(SubjectEditVm subjectEditVm, User lastModifier)
        {
            string imageFinalUrl = null;

            Subject currentSubject = await _unitOfWork.SubjectRepository.FindAsync(s=>s.Id== subjectEditVm.Id);

            if (subjectEditVm.NewImageUrl != null)
            {
                if (subjectEditVm.ImageUrl != null)
                {
                    var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Subject", subjectEditVm.ImageUrl);
                    System.IO.File.Delete(oldPathImage);
                }

                var extImage = Path.GetExtension(subjectEditVm.NewImageUrl.FileName);
                imageFinalUrl = Builder.BuildNameForImages() + extImage;
                var pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Subject", imageFinalUrl);

                await using var fileStream = new FileStream(pathImage, FileMode.Create);
                await subjectEditVm.NewImageUrl.CopyToAsync(fileStream);
            }
            else
            {
                imageFinalUrl = subjectEditVm.ImageUrl ?? "eeeeeeeeeeeeeeee.jpg";
            }

            var newSubject = new Subject()
            {
                Id = subjectEditVm.Id,
                CreateDate = currentSubject.CreateDate,
                LastModifyDate = DateTime.UtcNow,
                Creator = currentSubject.Creator,
                LastModifier = lastModifier,
                State = subjectEditVm.State,
                Title = subjectEditVm.Title,
                ImageUrl = imageFinalUrl,
                Films = currentSubject.Films
            };
            await _unitOfWork.SubjectRepository.Update(newSubject);
            await _unitOfWork.SaveAsync();
        }

        public async Task SubjectDeleteAction(SubjectDeleteVm subjectDeleteVm)
        {
            var filmsWithThisSubject = await _unitOfWork.FilmRepository.FindBySubjectAsync(subjectDeleteVm.Id, null);
            var newSubject = await _unitOfWork.SubjectRepository.FindByTitleAsync("عمومی");

            foreach (var film in filmsWithThisSubject)
            {
                var newFilm = new Film()
                {
                    Name = film.Name,
                    UploadDate = film.UploadDate,
                    Comments = film.Comments,
                    Description = film.Description,
                    FilmUrl = film.FilmUrl,
                    Id = film.Id,
                    Subject = newSubject,
                    ImageUrl = film.ImageUrl,
                    State = film.State,
                    LaseModifyDate = film.LaseModifyDate,
                    LastModifier = film.LastModifier,
                    LikeUsers = film.LikeUsers,
                    NumberOfViews = film.NumberOfViews,
                    Uploader = film.Uploader,
                    FilmTags = film.FilmTags
                };
                await _unitOfWork.FilmRepository.Update(newFilm);
            }

            await _unitOfWork.SubjectRepository.DeleteAsync(subjectDeleteVm.Id);

            if (subjectDeleteVm.ImageUrl != null)
            {
                var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Subject", subjectDeleteVm.ImageUrl);
                System.IO.File.Delete(oldPathImage);
            }

            await _unitOfWork.SaveAsync();
        }
    }
}
