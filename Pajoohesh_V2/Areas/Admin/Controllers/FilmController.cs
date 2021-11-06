using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Actions;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films;

namespace Pajoohesh_V2.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    [Area("Admin")]
    public class FilmController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IUserRepository _userRepository;

        private Films modelListFilms;
        private FilmActions filmActions;

        public FilmController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork, IWebHostEnvironment env, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            this.env = env;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> List(int pageNumber = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.All(c => c.Value != Constants.AdminClaim))
            {
                await _signInManager.SignOutAsync();
                return Redirect("/Identity/Account/SignIn");
            }

            modelListFilms = new Films(_unitOfWork,env, _userManager, _userRepository);
            var forListFilm = await modelListFilms.FilmListVms(true,pageNumber);
            return View(forListFilm);
        }

        public async Task<IActionResult> Add()
        {
            modelListFilms = new Films(_unitOfWork,env, _userManager, _userRepository);
            var forAddFilm = await modelListFilms.FilmAddVms();
            return View(forAddFilm);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<IActionResult> Add(FilmAddVm filmAddVm)
        {
            filmActions = new FilmActions(_userManager, _signInManager, _unitOfWork, env,_userRepository);
            var name = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);
            await filmActions.FilmAddAction(filmAddVm, user);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Edit(int id)
        {
            modelListFilms = new Films(_unitOfWork, env, _userManager, _userRepository);
            var forEditFilm = await modelListFilms.FilmEditVms(id,Request);
            return View(forEditFilm);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<IActionResult> Edit(FilmEditVm filmEditVm)
        {
            filmActions = new FilmActions(_userManager, _signInManager, _unitOfWork, env, _userRepository);
            var name = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);
            await filmActions.FilmEditAction(filmEditVm, filmEditVm.NewImageUrl, filmEditVm.NewFilmUrl, user);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(int id)
        {
            modelListFilms = new Films(_unitOfWork,env, _userManager, _userRepository);
            var forDeleteFilm = await modelListFilms.FilmDeleteVms(id);
            return View(forDeleteFilm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(FilmDeleteVm filmDeleteVm)
        {
            filmActions = new FilmActions(_userManager, _signInManager, _unitOfWork, env, _userRepository);
            await filmActions.FilmDeleteAction(filmDeleteVm);
            return RedirectToAction("List");
        }
    }
}