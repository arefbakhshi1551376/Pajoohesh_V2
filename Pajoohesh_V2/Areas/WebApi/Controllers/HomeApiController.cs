using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.Home;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.Home;

namespace Pajoohesh_V2.Areas.WebApi.Controllers
{
    [Area("WebApi")]
    public class HomeApiController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private UserManager<Model.Models.Main.User.User> _userManager;
        private IUserRepository _userRepository;

        public HomeApiController(IUnitOfWork unitOfWork, IWebHostEnvironment env,
            UserManager<Model.Models.Main.User.User> userManager, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            this.env = env;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List(SendHomeList sendHomeList)
        {
            Films modelListFilms = new Films(_unitOfWork, env, _userManager, _userRepository);
            var forListFilm = await modelListFilms.FilmApiListVms(false, sendHomeList.PageNumber, Request);
            ReceiveHomeList receiveHomeList = new ReceiveHomeList()
            {
                CurrentPageNumber = forListFilm.CurrentPageNumber,
                NumberOfPages = forListFilm.NumberOfPages,
                FilmListBaseApiVm = forListFilm.FilmListBaseApiVm,
            };
            return Json(receiveHomeList);
        }

        [HttpGet]
        public async Task<IActionResult> Watch(SendHomeWatch sendHomeWatch)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmWatchVm = await films.FilmWatchApiVms(sendHomeWatch.Id, sendHomeWatch.IsManagement, Request, sendHomeWatch.PageNumber);
            ReceiveHomeWatch receiveHomeWatch = new ReceiveHomeWatch()
            {
                Description = filmWatchVm.Description,
                Id = filmWatchVm.Id,
                Name = filmWatchVm.Name,
                Uploader = filmWatchVm.Uploader,
                FilmUrl = filmWatchVm.FilmUrl,
                ImageUrl = filmWatchVm.ImageUrl,
                UploadDate = filmWatchVm.UploadDate,
                UploaderImage = filmWatchVm.UploaderImage,
                CommentListVm = filmWatchVm.CommentListVm,
                NumberOfLikes = filmWatchVm.NumberOfLikes,
                NumberOfViews = filmWatchVm.NumberOfViews,
                OriginalUploadDate = filmWatchVm.OriginalUploadDate,
                RelatedFilmsVms = filmWatchVm.RelatedFilmsVms,
                SubjectNecessaryData = filmWatchVm.SubjectNecessaryData,
                TagNecessaryDataList = filmWatchVm.TagNecessaryDataList,
                UploadPastDateTime = filmWatchVm.UploadPastDateTime,
            };
            return Json(receiveHomeWatch);
        }

        [HttpGet]
        public async Task<IActionResult> ListByTag(SendHomeListByTag sendHomeListByTag)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListVms = await films.FilmListByTagApiVms(sendHomeListByTag.TagId, false, sendHomeListByTag.PageNumber,Request);
            ReceiveHomeListByTag receiveHomeListByTag = new ReceiveHomeListByTag()
            {
                CurrentPageNumber = filmListVms.CurrentPageNumber,
                NumberOfPages = filmListVms.NumberOfPages,
                TagNecessaryData = filmListVms.TagNecessaryData,
                FilmListBaseVms = filmListVms.FilmListBaseVms,
            };

            return Json(receiveHomeListByTag);
        }

        [HttpGet]
        public async Task<IActionResult> ListBySubject(SendHomeListBySubject sendHomeListBySubject)
        {
            Films films = new Films(_unitOfWork, env, _userManager, _userRepository);
            var filmListVms = await films.FilmListBySubjectApiVms(sendHomeListBySubject.SubjectId, false, sendHomeListBySubject.PageNumber,Request);
            ReceiveHomeListBySubject receiveHomeListBySubject = new ReceiveHomeListBySubject()
            {
                ImageUrl = filmListVms.ImageUrl,
                SubjectId = filmListVms.SubjectId,
                SubjectTitle = filmListVms.SubjectTitle,
                CurrentPageNumber = filmListVms.CurrentPageNumber,
                NumberOfPages = filmListVms.NumberOfPages,
                FilmListBaseVms = filmListVms.FilmListBaseVms,
            };
            return Json(receiveHomeListBySubject);
        }
    }
}