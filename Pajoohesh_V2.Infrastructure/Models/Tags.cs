using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Tags;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Models
{
    public class Tags
    {
        private IUnitOfWork _unitOfWork;

        public Tags(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TagListVm> TagListVms(int pageNumber)
        {
            var tags = await _unitOfWork.TagRepository.GetAllAsync();
            int numberOfFilmsInEachPage = Constants.DefaultNumberOfEntities;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfTags = await _unitOfWork.TagRepository.GetCount();
            var reminder = numberOfTags % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfTags / numberOfFilmsInEachPage;
            if (numberOfTags > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }
            var finalTags = tags.Skip(Constants.DefaultNumberOfEntities * (pageNumber - 1)).Take(Constants.DefaultNumberOfEntities);
            var currentTag = await _unitOfWork.TagRepository.FindByTitleAsync("عمومی");

            List<TagListBaseVm> tagListBaseVms = new List<TagListBaseVm>();

            foreach (var tag in finalTags)
            {
                if (tag == currentTag)
                {
                    continue;
                }

                tagListBaseVms.Add(new TagListBaseVm()
                {
                    Creator = tag.Creator.GetName(),
                    Id = tag.Id,
                    State = tag.State,
                    Title = tag.Title,
                    CreateDate = tag.CreateDate.ToPersianDate(),
                    LastModifier = tag.LastModifier.GetName(),
                    LastModifyDate = tag.LastModifyDate?.ToPersianDate(),
                    NumberOfFilms = (await _unitOfWork.FilmRepository.GetAllAsync()).Count(f => f.FilmTags.Select(t => t.TagId).Contains(tag.Id))
                });
            }

            TagListVm tagListVm = new TagListVm()
            {
                CurrentPageNumber = pageNumber,
                NumberOfPages = numberOfPages,
                TagListBaseVms = tagListBaseVms
            };
            return tagListVm;
        }

        public async Task<TagListVm> TagSearchVms(string statement, int pageNumber)
        {
            var tags = await _unitOfWork.TagRepository.GetAllAsync();
            int numberOfFilmsInEachPage = Constants.DefaultNumberOfEntities;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfTags = await _unitOfWork.TagRepository.GetCount();
            var reminder = numberOfTags % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfTags / numberOfFilmsInEachPage;
            if (numberOfTags > numberOfFilmsInEachPage)
            {
                if (reminder != 0)
                {
                    numberOfPages++;
                }
            }
            else
            {
                numberOfPages = 1;
            }
            var finalTags = tags.Skip(Constants.DefaultNumberOfEntities * (pageNumber - 1)).Take(Constants.DefaultNumberOfEntities);
            var currentTag = await _unitOfWork.TagRepository.FindByTitleAsync("عمومی");

            List<TagListBaseVm> tagListBaseVms = new List<TagListBaseVm>();

            foreach (var tag in finalTags)
            {
                if (tag == currentTag)
                {
                    continue;
                }

                tagListBaseVms.Add(new TagListBaseVm()
                {
                    Creator = tag.Creator.GetName(),
                    Id = tag.Id,
                    State = tag.State,
                    Title = tag.Title,
                    CreateDate = tag.CreateDate.ToPersianDate(),
                    LastModifier = tag.LastModifier.GetName(),
                    LastModifyDate = tag.LastModifyDate?.ToPersianDate(),
                    NumberOfFilms = (await _unitOfWork.FilmRepository.GetAllAsync()).Count(f => f.FilmTags.Select(t => t.TagId).Contains(tag.Id))
                });
            }

            TagListVm tagListVm = new TagListVm()
            {
                CurrentPageNumber = pageNumber,
                NumberOfPages = numberOfPages,
                TagListBaseVms = tagListBaseVms
            };
            return tagListVm;
        }

        public async Task<TagDeleteVm> TagDeleteVms(int id)
        {
            var tag = await _unitOfWork.TagRepository.FindAsync(t => t.Id == id);
            var numberOfFilms = (await _unitOfWork.FilmRepository.GetAllAsync(f => f.FilmTags.Select(ft=>ft.TagId).Contains(tag.Id))).Count();
            TagDeleteVm tagDeleteVm = new TagDeleteVm()
            {
                Id = tag.Id,
                Title = tag.Title,
                NumberOfFilms = numberOfFilms
            };

            return tagDeleteVm;
        }

        public async Task<TagEditVm> TagEditVms(int id)
        {
            var tag = await _unitOfWork.TagRepository.FindAsync(t => t.Id == id);
            TagEditVm subjectEditVm = new TagEditVm()
            {
                Id = tag.Id,
                Title = tag.Title,
                State = tag.State,
                CreateDate = tag.CreateDate.ToPersianDate(),
                LastModifyDate = tag.LastModifyDate?.ToPersianDate(),
                CreatorId = tag.Creator.Id,
                LastModifierId = tag.LastModifier?.Id,
                OriginalCreateDate = tag.CreateDate,
                OriginalLastModifyDate = tag.LastModifyDate
            };

            return subjectEditVm;
        }

        public async Task<string> GetTagsAsStringAsync(List<Tag> tags)
        {
            string tagsList = "";
            int counter = 1;
            var currentTag = await _unitOfWork.TagRepository.FindByTitleAsync("عمومی");
            foreach (var tag in tags)
            {
                if (tag == currentTag)
                {
                    continue;
                }

                tagsList += tag.Title;
                if (counter > 0 && counter < tags.Count)
                {
                    tagsList += " - ";
                }

                counter++;
            }

            return tagsList;
        }
    }
}