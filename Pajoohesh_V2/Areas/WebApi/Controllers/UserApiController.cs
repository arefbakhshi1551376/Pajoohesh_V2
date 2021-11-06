using System;
using System.Collections.Generic;
using System.IO;
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
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Users;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Areas.WebApi.Controllers
{
    [Area("WebApi")]
    [Authorize]
    public class UserApiController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;
        private IWebHostEnvironment env;
        private IHttpContextAccessor _httpContextAccessor;

        public UserApiController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork, IUserRepository userRepository, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            this.env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromBody] SendUserList sendUserList)
        {
            var users = await _userRepository.GetAllAsync();
            var loginUser = await _userManager.FindByNameAsync(User.Identity.Name);
            List<Model.Models.Main.User.User> adminUsers = new List<Model.Models.Main.User.User>();
            foreach (var user in users)
            {
                if (loginUser != user)
                {
                    if (await _userManager.IsInRoleAsync(user, Constants.AdminRole))
                    {
                        adminUsers.Add(user);
                    }
                }
            }
            UserListVm userListVms = new UserListVm();
            List<UserListBaseVm> userListVmBases = new List<UserListBaseVm>();
            var hostUrl = Request.Host.Value.ToString();
            foreach (var user in users)
            {
                if (loginUser != user)
                {
                    bool isAdmin = await _userManager.IsInRoleAsync(user, Constants.AdminRole);
                    //var finalPathImageForUser = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", user.ImageUrl);
                    var finalPathImageForUser = $"{hostUrl}/media/Images/User/{user.ImageUrl}";
                    var currentUserListVm = new UserListBaseVm()
                    {
                        Email = user.Email,
                        Id = user.Id,
                        FullName = user.GetName(),
                        UserName = user.UserName,
                        IsAccessValid = user.IsAccessValid,
                        IsAdmin = isAdmin,
                        RegisterDate = user.RegistryDate.ToPersianDate(),
                        ImageUrl = finalPathImageForUser
                    };
                    userListVmBases.Add(currentUserListVm);
                }
            }

            userListVms.UserListBaseVms = userListVmBases;

            ReceiveUserList receiveUserList = new ReceiveUserList()
            {
                CurrentPageNumber = userListVms.CurrentPageNumber,
                NumberOfPages = userListVms.NumberOfPages,
                UserListBaseVms = userListVms.UserListBaseVms
            };

            return Json(receiveUserList);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SendUserAdd sendUserAdd)
        {
            ReceiveUserAdd receiveUserAdd = new ReceiveUserAdd();
            try
            {
                UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
                await userActions.AddAsync(sendUserAdd);
                receiveUserAdd.IsAdded = true;
            }
            catch (Exception e)
            {
                receiveUserAdd.IsAdded = false;
            }
            return Json(receiveUserAdd);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(SendUserEditGet sendUserEditGet)
        {
            var currentUser = await _userRepository.FindAsync(sendUserEditGet.Id);
            var hostUrl = Request.Host.Value.ToString();
            //var finalPathImageForUser = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", currentUser.ImageUrl);
            var finalPathImageForUser = $"{hostUrl}/media/Images/Subject/{currentUser.ImageUrl}";
            ReceiveUserEditGet receiveUserEditGet = new ReceiveUserEditGet()
            {
                Email = currentUser.Email,
                Family = currentUser.Family,
                Id = currentUser.Id,
                Name = currentUser.Name,
                UserName = currentUser.UserName,
                IsAccessValid = currentUser.IsAccessValid,
                ImageUrl = finalPathImageForUser
            };
            return Json(receiveUserEditGet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SendUserEditPost sendUserEditPost)
        {
            ReceiveUserEditPost receiveUserEditPost = new ReceiveUserEditPost();
            try
            {
                UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
                var userName = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userName);
                await userActions.UpdateAsync(sendUserEditPost);
                if (user.Id == sendUserEditPost.Id)
                {
                    return RedirectToAction("SignOut", "Account", new { isFromApi = true });
                }

                receiveUserEditPost.IsEdited = true;
            }
            catch (Exception e)
            {
                receiveUserEditPost.IsEdited = false;
            }
            return Json(receiveUserEditPost);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(SendUserDeleteGet sendUserDeleteGet)
        {
            var currentUser = await _userRepository.FindAsync(sendUserDeleteGet.Id);
            var hostUrl = Request.Host.Value.ToString();
            var finalUserImage= $"{hostUrl}/media/Images/User/{currentUser.ImageUrl}";
            ReceiveUserDeleteGet receiveUserDeleteGet = new ReceiveUserDeleteGet()
            {
                Email = currentUser.Email,
                Family = currentUser.Family,
                Id = currentUser.Id,
                Name = currentUser.Name,
                UserName = currentUser.UserName,
                ImageUrl = finalUserImage
            };
            return Json(receiveUserDeleteGet);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] SendUserDeletePost sendUserDeletePost)
        {
            ReceiveUserDeletePost receiveUserDeletePost = new ReceiveUserDeletePost();
            try
            {
                var user = await _userRepository.FindAsync(sendUserDeletePost.Id);
                var isDeleted = await _userRepository.DeleteAsync(user);
                if (isDeleted)
                {
                    var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", user.ImageUrl);
                    System.IO.File.Delete(oldPathImage);
                }
                receiveUserDeletePost.IsDeleted = true;
            }
            catch (Exception e)
            {
                receiveUserDeletePost.IsDeleted = false;
            }
            return Json(receiveUserDeletePost);
        }

        [HttpGet]
        public async Task<IActionResult> AddRemoveAdmin(SendUserAddRemoveAdmin sendUserAddRemoveAdmin)
        {
            ReceiveUserAddRemoveAdmin receiveUserAddRemoveAdmin = new ReceiveUserAddRemoveAdmin();
            try
            {
                var user = await _userRepository.FindAsync(sendUserAddRemoveAdmin.Id);
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    await _userManager.AddClaimAsync(user, new Claim("UserType", "Operator"));
                    receiveUserAddRemoveAdmin.IsAdmin = true;
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                    await _userManager.RemoveClaimAsync(user, new Claim("UserType", "Operator"));
                    receiveUserAddRemoveAdmin.IsAdmin = false;
                }

                receiveUserAddRemoveAdmin.IsChange = true;
            }
            catch (Exception e)
            {
                receiveUserAddRemoveAdmin.IsChange = false;
            }
            return Json(receiveUserAddRemoveAdmin);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeAccess(SendUserChangeAccess sendUserChangeAccess)
        {
            ReceiveUserChangeAccess receiveUserChangeAccess = new ReceiveUserChangeAccess();
            try
            {
                UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
                var user = await _userRepository.FindAsync(sendUserChangeAccess.Id);
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
                    UserName = user.UserName
                };
                await userActions.UpdateAsync(userEditVm);
                receiveUserChangeAccess.IsChange = true;
                receiveUserChangeAccess.IsAccessValid = user.IsAccessValid;
            }
            catch (Exception e)
            {
                receiveUserChangeAccess.IsChange = false;
            }
            return Json(receiveUserChangeAccess);
        }
    }
}
