using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Comment;

namespace Pajoohesh_V2.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    [Area("Admin")]
    public class CommentController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;

        public CommentController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> List(int filmId, int pageNumber = 1)
        {
            Comments comments = new Comments(_unitOfWork);
            var currentUserName = User.Identity.Name;
            var currentUser = await _userManager.FindByNameAsync(currentUserName);
            bool isManagement = await _userManager.IsInRoleAsync(currentUser, Constants.AdminRole);
            var finalResult = await comments.CommentListVms(isManagement, pageNumber, filmId, Request);
            return PartialView("_Comment_List", finalResult);
        }

        /*        [HttpPost]
        public async Task<IActionResult> Edit()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            return RedirectToAction("List");
        }

        public IActionResult Details(int id)
        {
            return RedirectToAction("List");
        }*/
    }
}