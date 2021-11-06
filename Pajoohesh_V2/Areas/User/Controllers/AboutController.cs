using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.AboutUs;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.AboutUs;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Areas.User.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    [Area("User")]
    public class AboutController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;

        public AboutController(IUnitOfWork unitOfWork, UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AboutUsAddVm aboutUsAddVm)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            AboutUs aboutUs = new AboutUs()
            {
                Title = aboutUsAddVm.Title,
                CreateDate = DateTime.UtcNow,
                Creator = currentUser,
                LastModifier = null,
                LastModifyDate = null,
                Text = aboutUsAddVm.Text
            };
            await _unitOfWork.AboutUsRepository.AddAsync(aboutUs);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("AboutUs");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentAboutUs = await _unitOfWork.AboutUsRepository.FindAsync(au => au.Id == id);
            AboutUsEditVm aboutUsEditVm = new AboutUsEditVm()
            {
                Title = currentAboutUs.Title,
                Text = currentAboutUs.Text,
                CreateDate = currentAboutUs.CreateDate.ToPersianDate(),
                CreatorId = currentAboutUs.Creator.Id,
                Id = currentAboutUs.Id,
                OriginalCreateDate = currentAboutUs.CreateDate
            };
            return View(aboutUsEditVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AboutUsEditVm aboutUsEditVm)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            AboutUs aboutUs = new AboutUs()
            {
                Text = aboutUsEditVm.Text,
                CreateDate = aboutUsEditVm.OriginalCreateDate,
                Id = aboutUsEditVm.Id,
                Creator = await _userManager.FindByIdAsync(aboutUsEditVm.CreatorId),
                LastModifier = currentUser,
                LastModifyDate = DateTime.UtcNow,
                Title = aboutUsEditVm.Title
            };
            await _unitOfWork.AboutUsRepository.Update(aboutUs);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("AboutUs");
        }

        [AllowAnonymous]
        public async Task<IActionResult> AboutUs()
        {
            var currentAboutUs = (await _unitOfWork.AboutUsRepository.GetAllAsync()).FirstOrDefault();
            AboutUsShowVm aboutUsShowVm;
            if (currentAboutUs == null)
            {
                aboutUsShowVm = new AboutUsShowVm()
                {
                    Id = 0
                };
            }
            else
            {
                aboutUsShowVm = new AboutUsShowVm()
                {
                    Text = currentAboutUs.Text,
                    Title = currentAboutUs.Title,
                    CreateDate = currentAboutUs.CreateDate.ToPersianDate(),
                    Creator = currentAboutUs.Creator.GetName(),
                    Id = currentAboutUs.Id,
                    LastModifyDate = currentAboutUs.LastModifyDate?.ToPersianDate(),
                    LastModifier = currentAboutUs.LastModifier?.GetName()
                };
            }
            return View(aboutUsShowVm);
        }
    }
}
