using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.Models.Relationship;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Actions
{
    public class FilmActions : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IUserRepository _userRepository;

        public FilmActions(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork,
            IWebHostEnvironment env, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            this.env = env;
            _userRepository = userRepository;
        }

        public async Task FilmAddAction(FilmAddVm filmAddVm, User user)
        {
            var tags = await GetTagListForAddFilm(filmAddVm.Tags, user);

            string imageFinalUrl = null;
            string videoFinalUrl = null;
            string pathImage = null;

            if (filmAddVm.ImageUrl == null)
            {
                imageFinalUrl = "eeeeeeeeeeeeeeee.jpg";
            }
            else
            {
                var extImage = Path.GetExtension(filmAddVm.ImageUrl.FileName);
                imageFinalUrl = Builder.BuildNameForImages() + extImage;
                pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Film", imageFinalUrl);
            }

            var filmData = filmAddVm.FilmUrl.FirstOrDefault();
            string extFilm = Path.GetExtension(filmData?.FileName);
            videoFinalUrl = Builder.BuildNameForVideos() + extFilm;
            var pathFilm = Path.Combine(env.WebRootPath + "\\Media\\Videos", videoFinalUrl);

            var isFilmAddSuccess = await IsFilmAddSuccess(filmAddVm, user, videoFinalUrl, imageFinalUrl, tags);
            if (isFilmAddSuccess)
            {
                if (pathImage != null)
                {
                    await using var fileStreamForImage = new FileStream(pathImage, FileMode.Create);
                    await filmAddVm.ImageUrl.CopyToAsync(fileStreamForImage);
                }

                await using var fileStreamForFilm = new FileStream(pathFilm, FileMode.Create);
                await filmData?.CopyToAsync(fileStreamForFilm);
            }
        }

        private async Task<bool> IsFilmAddSuccess(FilmAddVm filmAddVm, User user, string videoFinalUrl, string imageFinalUrl,
            List<Tag> tags)
        {
            try
            {
                var currentFilm = new Film()
                {
                    State = filmAddVm.State,
                    Comments = null,
                    Description = filmAddVm.Description,
                    FilmUrl = videoFinalUrl,
                    ImageUrl = imageFinalUrl,
                    LaseModifyDate = null,
                    LastModifier = null,
                    LikeUsers = null,
                    Name = filmAddVm.Name.Trim(),
                    NumberOfViews = 0,
                    Subject = await MakeSubjectAsync(filmAddVm, user),
                    FilmTags = null,
                    UploadDate = DateTime.UtcNow,
                    Uploader = user
                };

                await _unitOfWork.FilmRepository.AddAsync(currentFilm);

                List<FilmTag> filmTags = new List<FilmTag>();

                foreach (var tag in tags)
                {
                    filmTags.Add(new FilmTag()
                    {
                        Tag = tag,
                        Film = currentFilm,
                        FilmId = currentFilm.Id,
                        TagId = tag.Id
                    });
                }

                currentFilm.FilmTags = filmTags;

                await _unitOfWork.SaveAsync();

                NotifierOfNewMovies notifierOfNewMovies = new NotifierOfNewMovies(_userRepository);
                await notifierOfNewMovies.Inform(currentFilm.Name, "www.yejaii.com");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<Subject> MakeSubjectAsync(FilmAddVm filmAddVm, User user)
        {
            var defaultBuilder = new DefaultBuilder(_unitOfWork);
            await defaultBuilder.BuildDefaultSubject(user);
            return await _unitOfWork.SubjectRepository.FindAsync(s => s.Id == filmAddVm.SubjectId);
        }

        public async Task FilmEditAction(FilmEditVm filmEditVm, IFormFile ImageUrl, List<IFormFile> FilmUrl, User user)
        {
            var tags = await GetTagListForAddFilm(filmEditVm.Tags, user);

            string imageFinalUrl = null;
            string videoFinalUrl = null;

            if (ImageUrl != null)
            {
                var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Film", filmEditVm.ImageUrl);
                System.IO.File.Delete(oldPathImage);

                var extImage = Path.GetExtension(ImageUrl.FileName);
                imageFinalUrl = Builder.BuildNameForImages() + extImage;
                var pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Film", imageFinalUrl);

                await using var fileStream = new FileStream(pathImage, FileMode.Create);
                await ImageUrl.CopyToAsync(fileStream);
            }
            else
            {
                imageFinalUrl = filmEditVm.ImageUrl;
            }


            if (FilmUrl.Count != 0)
            {
                foreach (var formFile in FilmUrl)
                {
                    if (filmEditVm.FilmUrl != null)
                    {
                        var oldPathFilm = Path.Combine(env.WebRootPath + "\\Media\\Videos", filmEditVm.FilmUrl);
                        System.IO.File.Delete(oldPathFilm);
                    }


                    string extFilm = Path.GetExtension(formFile.FileName);
                    videoFinalUrl = Builder.BuildNameForVideos() + extFilm;
                    var pathFilm = Path.Combine(env.WebRootPath + "\\Media\\Videos", videoFinalUrl);
                    await using var fileStream = new FileStream(pathFilm, FileMode.Create);
                    await formFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                videoFinalUrl = filmEditVm.FilmUrl;
            }

            List<FilmTag> filmTags = new List<FilmTag>();

            foreach (var tag in tags)
            {
                filmTags.Add(new FilmTag()
                {
                    Tag = tag,
                    Film = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == filmEditVm.Id),
                    FilmId = filmEditVm.Id,
                    TagId = tag.Id
                });
            }

            List<FilmLikeUser> likeUsers = new List<FilmLikeUser>();
            foreach (var currentUser in filmEditVm.LikeUsers)
            {
                FilmLikeUser currentFilmLikeUser = new FilmLikeUser()
                {
                    User = user,
                    UserId = user.Id,
                    FilmId = filmEditVm.Id,
                    Film = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == filmEditVm.Id)
                };
                likeUsers.Add(currentFilmLikeUser);
            }

            var currentFilm = new Film()
            {
                State = filmEditVm.State,
                Comments = filmEditVm.Comments,
                Description = filmEditVm.Description,
                FilmUrl = videoFinalUrl,
                ImageUrl = imageFinalUrl,
                LaseModifyDate = DateTime.UtcNow,
                LastModifier = user,
                LikeUsers = likeUsers,
                Name = filmEditVm.Name,
                NumberOfViews = filmEditVm.NumberOfViews,
                Subject = await _unitOfWork.SubjectRepository.FindAsync(s => s.Id == filmEditVm.SubjectId),
                UploadDate = filmEditVm.OriginalUploadDate,
                Uploader = await _userManager.FindByIdAsync(filmEditVm.UploaderId),
                Id = filmEditVm.Id,
                FilmTags = filmTags
            };
            await _unitOfWork.FilmRepository.Update(currentFilm);
            await _unitOfWork.SaveAsync();
        }

        private async Task<List<Tag>> GetTagListForAddFilm(string tagStringList, User user)
        {
            var newTagStringList = tagStringList.Split("-");

            List<Tag> finalTags = new List<Tag>();
            TagActions tagActions = new TagActions(_unitOfWork, _userManager);
            foreach (var tagString in newTagStringList)
            {
                var currentTag = await tagActions.TagAddAction(tagString, user);
                finalTags.Add(currentTag);
            }

            return finalTags;
        }

        public async Task FilmDeleteAction(FilmDeleteVm filmDeleteVm)
        {
            var currentFilm = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == filmDeleteVm.Id);

            var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Film", currentFilm.ImageUrl);
            System.IO.File.Delete(oldPathImage);

            var oldPathFilm = Path.Combine(env.WebRootPath + "\\Media\\Videos", currentFilm.FilmUrl);
            System.IO.File.Delete(oldPathFilm);

            await _unitOfWork.FilmRepository.DeleteAsync(filmDeleteVm.Id);
            await _unitOfWork.SaveAsync();
        }
        
        public async Task FilmDeleteAction(int id)
        {
            var currentFilm = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == id);

            var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\Film", currentFilm.ImageUrl);
            System.IO.File.Delete(oldPathImage);

            var oldPathFilm = Path.Combine(env.WebRootPath + "\\Media\\Videos", currentFilm.FilmUrl);
            System.IO.File.Delete(oldPathFilm);

            await _unitOfWork.FilmRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}