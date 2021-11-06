using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Actions;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.Models.Main.ContactUs;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ContactUs;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = Constants.AdminRole)]
    public class ContactController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;

        private Contacts contactsModel;
        private ContactActions contactActions;

        public ContactController(IUnitOfWork unitOfWork, UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> List(int pageNumber = 1)
        {
            contactsModel = new Contacts(_unitOfWork);
            var contactUsList = await contactsModel.ContactListVms(pageNumber);
            return View(contactUsList);
        }

        [AllowAnonymous]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Add(ContactUsAddVm contactUsAddVm)
        {
            contactActions = new ContactActions(_userManager, _signInManager, _unitOfWork);
            await contactActions.ContactAddAction(contactUsAddVm);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Reply(int id)
        {
            contactsModel = new Contacts(_unitOfWork);
            var contactUsReplyVm = await contactsModel.ContactReplyVms(id);
            return View(contactUsReplyVm);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(ContactUsReplyVm contactUsReplyVm)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            contactActions = new ContactActions(_userManager, _signInManager, _unitOfWork);
            await contactActions.ContactReplyAction(contactUsReplyVm, currentUser);

            return RedirectToAction("List");
        }

        public async Task<IActionResult> Details(int id)
        {
            contactsModel = new Contacts(_unitOfWork);
            var contactUsDetailsVm = await contactsModel.ContactDetailsVms(id);
            return View(contactUsDetailsVm);
        }
    }
}
