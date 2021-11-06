using System.Collections.Generic;
using System.Security.Claims;
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
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Users;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUserRepository _userRepository;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment env;
        private IHttpContextAccessor _httpContextAccessor;

        public UserController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUserRepository userRepository, IWebHostEnvironment env, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            this.env = env;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> List(int pageNumber = 1)
        {
            int numberOfFilmsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfFilmsInEachPage;
            int numberOfUsers = (await _userRepository.GetCount()) - 1;
            var reminder = numberOfUsers % numberOfFilmsInEachPage;
            var numberOfPages = (int)numberOfUsers / numberOfFilmsInEachPage;
            if (numberOfUsers > numberOfFilmsInEachPage)
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

            var users = await _userRepository.GetAllAsync();
            var loginUser = await _userManager.FindByNameAsync(User.Identity.Name);

            List<UserListBaseVm> userListVmBases = new List<UserListBaseVm>();
            foreach (var user in users)
            {
                if (loginUser != user)
                {
                    bool isAdmin = await _userManager.IsInRoleAsync(user, Constants.AdminRole);
                    var currentUserListVm = new UserListBaseVm()
                    {
                        Email = user.Email,
                        FullName = user.GetName(),
                        Id = user.Id,
                        UserName = user.UserName,
                        IsAccessValid = user.IsAccessValid,
                        IsAdmin = isAdmin,
                        RegisterDate = user.RegistryDate.ToPersianDate()
                    };
                    userListVmBases.Add(currentUserListVm);
                }
            }

            UserListVm userListVms = new UserListVm()
            {
                CurrentPageNumber = pageNumber,
                NumberOfPages = numberOfPages,
                UserListBaseVms = userListVmBases
            };

            return View(userListVms);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserAddVm userAddVm)
        {
            UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            await userActions.AddAsync(userAddVm);
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var currentUser = await _userRepository.FindAsync(id);
            UserEditVm userEditVm = new UserEditVm()
            {
                Email = currentUser.Email,
                Family = currentUser.Family,
                Id = currentUser.Id,
                Name = currentUser.Name,
                UserName = currentUser.UserName,
                IsAccessValid = currentUser.IsAccessValid,
                ImageUrl = currentUser.ImageUrl
            };
            return View(userEditVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditVm userEditVm)
        {
            UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            await userActions.UpdateAsync(userEditVm);
            if (user.Id == userEditVm.Id)
            {
                return Redirect("/Identity/Account/SignOut");
            }
            return RedirectToAction("List");
        }

        //public async Task<IActionResult> Delete(string id)
        //{
        //    var currentUser = await _userRepository.FindAsync(id);
        //    UserDeleteVm userDeleteVm = new UserDeleteVm()
        //    {
        //        Email = currentUser.Email,
        //        Family = currentUser.Family,
        //        Id = currentUser.Id,
        //        Name = currentUser.Name,
        //        UserName = currentUser.UserName
        //    };
        //    return View(userDeleteVm);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Delete(UserDeleteVm userDeleteVm)
        //{
        //    UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
        //    var user = await _userRepository.FindAsync(userDeleteVm.Id);
        //    await userActions.DeleteAsync(user);
        //    return RedirectToAction("List");
        //}

        public async Task<IActionResult> Delete(string id)
        {
            UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            var user = await _userRepository.FindAsync(id);
            await userActions.DeleteAsync(user);
            return RedirectToAction("List");
        }

        public IActionResult Details(int id)
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> AddRemoveAdmin(string id)
        {
            var user = await _userRepository.FindAsync(id);
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                await _userManager.AddClaimAsync(user, new Claim("UserType", Constants.AdminClaim));
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.RemoveClaimAsync(user, new Claim("UserType", Constants.AdminClaim));
            }
            return RedirectToAction("List");
        }

        public async Task<IActionResult> ChangeAccess(string id)
        {
            UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            var user = await _userRepository.FindAsync(id);
            UserEditVm userEditVm = new UserEditVm()
            {
                ConfirmPassword = null,
                Email = user.Email,
                Family = user.Family,
                Id = user.Id,
                IsAccessValid = !user.IsAccessValid,
                Name = user.Name,
                NewPassword = null,
                OldPassword = null,
                UserName = user.UserName,
                ImageUrl = user.ImageUrl
            };
            await userActions.UpdateAsync(userEditVm);
            return RedirectToAction("List");
        }
    }
}
