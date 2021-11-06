using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Tags;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Actions
{
    public class TagActions : Controller
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<User> _userManager;

        public TagActions(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task TagAddAction(TagAddVm tagAddVm, User user)
        {
            var defaultBuilder = new DefaultBuilder(_unitOfWork);
            await defaultBuilder.BuildDefaultTag(user);
            var tagTitles = (await _unitOfWork.TagRepository.GetAllAsync()).Select(t => t.Title);
            if (!tagTitles.Contains(tagAddVm.Title.Trim()))
            {
                var currentTag = new Tag()
                {
                    Creator = user,
                    State = tagAddVm.State,
                    Title = tagAddVm.Title.Trim(),
                    CreateDate = DateTime.UtcNow,
                    LastModifier = null,
                    LastModifyDate = null
                };
                await _unitOfWork.TagRepository.AddAsync(currentTag);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<Tag> TagAddAction(string tagTitle, User user)
        {
            var defaultBuilder = new DefaultBuilder(_unitOfWork);
            await defaultBuilder.BuildDefaultTag(user);
            var tagTitles = (await _unitOfWork.TagRepository.GetAllAsync()).Select(t => t.Title);
            Tag finalTag;
            if (!tagTitles.Contains(tagTitle.Trim()))
            {
                var currentTag = new Tag()
                {
                    Creator = user,
                    State = State.Enable,
                    Title = tagTitle.Trim(),
                    CreateDate = DateTime.UtcNow,
                    LastModifier = null,
                    LastModifyDate = null
                };
                await _unitOfWork.TagRepository.AddAsync(currentTag);
                await _unitOfWork.SaveAsync();
            }
            finalTag = await _unitOfWork.TagRepository.FindByTitleAsync(tagTitle.Trim());
            return finalTag;
        }

        public async Task TagEditAction(TagEditVm tagEditVm, User lastModifier)
        {
            Tag currentTag = await _unitOfWork.TagRepository.FindAsync(t => t.Id == tagEditVm.Id);
            var newTag = new Tag()
            {
                Id = tagEditVm.Id,
                CreateDate = currentTag.CreateDate,
                LastModifyDate = DateTime.UtcNow,
                Creator = currentTag.Creator,
                LastModifier = lastModifier,
                State = tagEditVm.State,
                Title = tagEditVm.Title,
            };
            await _unitOfWork.TagRepository.Update(newTag);
            await _unitOfWork.SaveAsync();
        }

        public async Task TagDeleteAction(TagDeleteVm tagDeleteVm)
        {
            var filmsWithOneTag = await _unitOfWork.FilmRepository.FindWithOneTagAsync(tagDeleteVm.Id);
            var currentTag = await _unitOfWork.TagRepository.FindByTitleAsync("عمومی");
            List<Tag> tagList = new List<Tag>();
            tagList.Add(currentTag);

            foreach (var film in filmsWithOneTag)
            {
                Film newFilm = new Film()
                {
                    Name = film.Name,
                    UploadDate = film.UploadDate,
                    Comments = film.Comments,
                    Description = film.Description,
                    FilmUrl = film.FilmUrl,
                    Id = film.Id,
                    //FilmTags Need Modify Tags = tagList,
                    Subject = film.Subject,
                    ImageUrl = film.ImageUrl,
                    LaseModifyDate = film.LaseModifyDate,
                    LastModifier = film.LastModifier,
                    LikeUsers = film.LikeUsers,
                    NumberOfViews = film.NumberOfViews,
                    State = film.State,
                    Uploader = film.Uploader
                };
                await _unitOfWork.FilmRepository.Update(newFilm);
            }

            await _unitOfWork.SaveAsync();

            await _unitOfWork.TagRepository.DeleteAsync(tagDeleteVm.Id);
            await _unitOfWork.SaveAsync();
        }
    }
}
