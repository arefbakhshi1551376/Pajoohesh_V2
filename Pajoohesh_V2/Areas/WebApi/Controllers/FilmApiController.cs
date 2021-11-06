using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Actions;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.Film;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.Film;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films;

namespace Pajoohesh_V2.Areas.WebApi.Controllers
{
    [Area("WebApi")]
    [Authorize]
    public class FilmApiController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IUserRepository _userRepository;

        private Films modelListFilms;
        private FilmActions filmActions;

        public FilmApiController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork, IWebHostEnvironment env, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            this.env = env;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> List(SendFilmList sendFilmList)
        {
            ReceiveFilmList receiveFilmList = new ReceiveFilmList();
            
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.All(c => c.Value != Constants.AdminClaim))
            {
                await _signInManager.SignOutAsync();
                return Json(Ok());
            }

            modelListFilms = new Films(_unitOfWork,env, _userManager, _userRepository);
            var forListFilm = await modelListFilms.FilmApiListVms(sendFilmList.IsManagement, sendFilmList.PageNumber, Request);

            receiveFilmList.FilmListBaseApiVm = forListFilm.FilmListBaseApiVm;
            receiveFilmList.NumberOfPages = forListFilm.NumberOfPages;
            receiveFilmList.CurrentPageNumber = forListFilm.CurrentPageNumber;

            return Json(receiveFilmList);
        }
        
        [HttpGet]
        public async Task<IActionResult> Delete(SendFilmDeleteGet sendFilmDeleteGet)
        {
            modelListFilms = new Films(_unitOfWork,env, _userManager, _userRepository);
            var forDeleteFilm = await modelListFilms.FilmDeleteApiVms(sendFilmDeleteGet.Id);
            ReceiveFilmDeleteGet receiveFilmDeleteGet = new ReceiveFilmDeleteGet()
            {
                Description = forDeleteFilm.Description,
                Id = forDeleteFilm.Id,
                Name = forDeleteFilm.Name,
                FilmUrl = forDeleteFilm.FilmUrl,
                UploadDate = forDeleteFilm.UploadDate,
                SubjectNecessaryData = forDeleteFilm.SubjectNecessaryData,
                UploadPastDate = forDeleteFilm.UploadPastDate,
                TagNecessaryDataList = forDeleteFilm.TagNecessaryDataList,
            };
            return Json(receiveFilmDeleteGet);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] SendFilmDeletePost sendFilmDeletePost)
        {
            filmActions = new FilmActions(_userManager, _signInManager, _unitOfWork, env, _userRepository);
            ReceiveFilmDeletePost receiveFilmDeletePost = new ReceiveFilmDeletePost();
            try
            {
                await filmActions.FilmDeleteAction(sendFilmDeletePost.Id);
                receiveFilmDeletePost.IsDeleted = true;
            }
            catch (Exception e)
            {
                receiveFilmDeletePost.IsDeleted = false;
            }
            return Json(receiveFilmDeletePost);
        }
    }
}
