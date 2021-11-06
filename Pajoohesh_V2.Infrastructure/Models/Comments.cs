using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Comments;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Models
{
    public class Comments
    {
        private IUnitOfWork _unitOfWork;

        public Comments(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentListVm> CommentListVms(bool isInManagement, int pageNumber, int filmId, HttpRequest httpRequest)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfFilms = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfFilms % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfFilms / numberOfFilmsInEachPage;
            if (numberOfFilms > numberOfFilmsInEachPage)
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

            var currentFilm = await _unitOfWork.FilmRepository.FindAsync(f => f.Id == filmId);
            var comments = (await _unitOfWork.CommentRepository.GetAllAsync(c => c.Film.Id == filmId)).OrderByDescending(c=>c.CreateDate);

            List<CommentListBaseVm> commentListBaseVms = new List<CommentListBaseVm>();

            foreach (var comment in comments)
            {
                var finalCommentCreatorImage = comment.Creator.GetUserImageUrl(httpRequest);
                CommentListBaseVm commentListBaseVm = new CommentListBaseVm()
                {
                    CreateDate = comment.CreateDate.ToPersianDate(),
                    Creator = comment.Creator.GetName(),
                    Id = comment.Id,
                    LaseModifyDate = comment.LaseModifyDate?.ToPersianDate(),
                    State = comment.State,
                    Text = comment.Text,
                    CreatorImage = finalCommentCreatorImage,
                    PastCreateDate = comment.CreateDate.CallPastTime(),
                    PastLaseModifyDate = comment.LaseModifyDate?.CallPastTime()
                };
                commentListBaseVms.Add(commentListBaseVm);
            }

            CommentListVm finalResult = null;

            if (isInManagement)
            {
                finalResult = new CommentListVm()
                {
                    CommentListBaseVms =commentListBaseVms,
                    CurrentPageNumber = pageNumber,
                    NumberOfPages = numberOfPages
                };
            }
            else
            {
                finalResult = new CommentListVm()
                {
                    CommentListBaseVms = commentListBaseVms.Where(c=>c.State==State.Enable).ToList(),
                    CurrentPageNumber = pageNumber,
                    NumberOfPages = numberOfPages
                };
            }

            return finalResult;
        }
    }
}
