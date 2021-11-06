using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Models
{
    public class Subjects
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;

        public Subjects(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            this.env = env;
        }

        public async Task<SubjectListVm> SubjectListVms(int pageNumber, string hostUrl)
        {
            var subjects = await _unitOfWork.SubjectRepository.GetAllAsync();
            int numberOfFilmsInEachPage = Constants.DefaultNumberOfEntities;

            int numberOfSubjects = await _unitOfWork.SubjectRepository.GetCount();
            var reminder = numberOfSubjects % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfSubjects / numberOfFilmsInEachPage;
            if (numberOfSubjects > numberOfFilmsInEachPage)
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
            var finalSubjects = subjects.Skip(Constants.DefaultNumberOfEntities * (pageNumber - 1)).Take(Constants.DefaultNumberOfEntities);
            var currentSubject = await _unitOfWork.SubjectRepository.FindByTitleAsync("عمومی");

            List<SubjectListBaseVm> subjectListBaseVms = new List<SubjectListBaseVm>();

            foreach (var subject in finalSubjects)
            {
                if (subject == currentSubject)
                {
                    continue;
                }

                var finalSubjectImage = $"{hostUrl}/media/Images/Subject/{subject.ImageUrl}";

                SubjectListBaseVm subjectListBaseVm = new SubjectListBaseVm()
                {
                    Creator = subject.Creator.GetName(),
                    Id = subject.Id,
                    State = subject.State,
                    Title = subject.Title,
                    CreateDate = subject.CreateDate.ToPersianDate(),
                    LastModifier = subject.LastModifier.GetName(),
                    LastModifyDate = subject.LastModifyDate?.ToPersianDate(),
                    NumberOfFilms = (await _unitOfWork.FilmRepository.GetAllAsync()).Count(f => f.Subject == subject),
                    ImageUrl = finalSubjectImage
                };

                subjectListBaseVms.Add(subjectListBaseVm);
            }

            SubjectListVm subjectListVm = new SubjectListVm()
            {
                CurrentPageNumber = pageNumber,
                NumberOfPages = numberOfPages,
                SubjectListBaseVms = subjectListBaseVms
            };
            return subjectListVm;
        }

        public async Task<SubjectDeleteVm> SubjectDeleteVms(int id)
        {
            var subject = await _unitOfWork.SubjectRepository.FindAsync(s => s.Id == id);
            var numberOfFilms = (await _unitOfWork.FilmRepository.GetAllAsync(f => f.Subject == subject)).Count();

            SubjectDeleteVm subjectDeleteVm = new SubjectDeleteVm()
            {
                Id = subject.Id,
                Title = subject.Title,
                NumberOfFilms = numberOfFilms,
                ImageUrl = subject.ImageUrl
            };

            return subjectDeleteVm;
        }

        public async Task<SubjectDeleteVm> SubjectDeleteApiVms(int id, string hostUrl)
        {
            var subject = await _unitOfWork.SubjectRepository.FindAsync(s => s.Id == id);
            var numberOfFilms = (await _unitOfWork.FilmRepository.GetAllAsync(f => f.Subject == subject)).Count();
            //var finalPathImageForSubject = Path.Combine(env.WebRootPath + "\\Media\\Images\\Subject", subject.ImageUrl);
            var finalPathImageForSubject = $"{hostUrl}/media/Images/Subject/{subject.ImageUrl}";
            SubjectDeleteVm subjectDeleteVm = new SubjectDeleteVm()
            {
                Id = subject.Id,
                Title = subject.Title,
                NumberOfFilms = numberOfFilms,
                ImageUrl = finalPathImageForSubject
            };

            return subjectDeleteVm;
        }

        public async Task<SubjectEditVm> SubjectEditVms(int id, string hostUrl)
        {
            var subject = await _unitOfWork.SubjectRepository.FindAsync(s => s.Id == id);
            //var finalPathImageForSubject = Path.Combine(env.WebRootPath + "\\Media\\Images\\Subject", subject.ImageUrl);
            var finalPathImageForSubject = $"{hostUrl}/media/Images/Subject/{subject.ImageUrl}";
            SubjectEditVm subjectEditVm = new SubjectEditVm()
            {
                Id = subject.Id,
                Title = subject.Title,
                State = subject.State,
                CreateDate = subject.CreateDate.ToPersianDate(),
                LastModifyDate = subject.LastModifyDate?.ToPersianDate(),
                CreatorId = subject.Creator.Id,
                LastModifierId = subject.LastModifier?.Id,
                OriginalCreateDate = subject.CreateDate,
                OriginalLastModifyDate = subject.LastModifyDate,
                ImageUrl = finalPathImageForSubject
            };

            return subjectEditVm;
        }
    }
}