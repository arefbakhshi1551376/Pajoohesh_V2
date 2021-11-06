using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Actions;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Accounts;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ForgetPasswords;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Users;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Areas.Identity.Controllers
{
    [Authorize]
    [Area("Identity")]
    public class AccountController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUserRepository _userRepository;
        private IWebHostEnvironment env;
        private ILogger<AccountController> _logger;
        private IUnitOfWork _unitOfWork;
        private IHttpContextAccessor _httpContextAccessor;

        public AccountController(UserManager<Model.Models.Main.User.User> userManager, SignInManager<Model.Models.Main.User.User> signInManager, IUserRepository userRepository, IWebHostEnvironment env, ILogger<AccountController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            this.env = env;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Signin(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin(SignInVm signInVm, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!string.IsNullOrEmpty(signInVm.UserName) && !string.IsNullOrEmpty(signInVm.PassWord))
            {
                var userByName = await _userManager.FindByNameAsync(signInVm.UserName);
                var userByEmail = await _userManager.FindByEmailAsync(signInVm.UserName);
                Model.Models.Main.User.User finalUser;
                if (userByName != null)
                {
                    finalUser = userByName;
                }
                else if (userByEmail != null)
                {
                    finalUser = userByEmail;
                }
                else
                {
                    finalUser = null;
                }
                if (finalUser != null)
                {
                    if (finalUser.IsAccessValid)
                    {
                        var result = await _signInManager.PasswordSignInAsync(finalUser, signInVm.PassWord, signInVm.RememberMe, false);
                        if (result.Succeeded)
                        {
                            var claimsCheck = await _userManager.GetClaimsAsync(finalUser);
                            var isAdmin = await _userManager.IsInRoleAsync(finalUser, Constants.AdminRole);
                            if (isAdmin)
                            {
                                if (returnUrl != null)
                                {
                                    if (Url.IsLocalUrl(returnUrl))
                                    {
                                        return Redirect(returnUrl);
                                    }
                                    else
                                    {
                                        return Redirect("/Admin/Film/List");
                                    }
                                }
                                else
                                {
                                    return Redirect("/Admin/Film/List");
                                }
                            }
                            else
                            {
                                if (claimsCheck.Any(c => c.Value == Constants.AdminClaim))
                                {
                                    if (returnUrl != null)
                                    {
                                        if (Url.IsLocalUrl(returnUrl))
                                        {
                                            return Redirect(returnUrl);
                                        }
                                        else
                                        {
                                            return Redirect("/Admin/Film/List");
                                        }
                                    }
                                    else
                                    {
                                        return Redirect("/Admin/Film/List");
                                    }
                                }
                                else
                                {
                                    if (claimsCheck.Any(c => c.Value == Constants.UserClaim))
                                    {
                                        if (returnUrl != null)
                                        {
                                            if (Url.IsLocalUrl(returnUrl))
                                            {
                                                return Redirect(returnUrl);
                                            }
                                            else
                                            {
                                                return Redirect("/");
                                            }
                                        }
                                        else
                                        {
                                            return Redirect("/");
                                        }
                                    }
                                    else
                                    {
                                        signInVm.Message = "مشکلی پیش آمده است!";
                                        return RedirectToAction("Signup", "Account", signInVm);
                                    }
                                }
                            }
                        }
                        else
                        {
                            signInVm.Message = "مشکلی پیش آمده است!";
                            return View(signInVm);
                        }
                    }
                    else
                    {
                        signInVm.Message = "دسترسی شما به سایت مجاز نیست!";
                        return View(signInVm);
                    }
                }
                else
                {
                    signInVm.Message = "چنین کاربری وجود ندارد!";
                    return View(signInVm);
                }
            }
            else
            {
                return View(signInVm);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public bool CheckUserNameAndEmail(string userName, string email)
        {
            bool isRepeated = false;
            var userEmails = _userManager.Users.Select(u => u.Email);
            var userNames = _userManager.Users.Select(u => u.UserName);
            if (userEmails.Contains(email) || userNames.Contains(userName))
            {
                isRepeated = true;
            }
            return isRepeated;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(SignUpVm signUpVm)
        {
            var userEmails = _userManager.Users.Select(u => u.Email);
            var userNames = _userManager.Users.Select(u => u.UserName);
            if (userEmails.Contains(signUpVm.Email) || userNames.Contains(signUpVm.UserName))
            {
                signUpVm.Message = "چنین کاربری قبلا ثبت نام کرده است!";
                return View(signUpVm);
            }
            else
            {
                if (!string.IsNullOrEmpty(signUpVm.Password.Trim())
                    && !string.IsNullOrEmpty(signUpVm.ConfirmPassword.Trim())
                    && signUpVm.Password.Trim().Equals(signUpVm.ConfirmPassword.Trim())
                    && !string.IsNullOrEmpty(signUpVm.Email.Trim())
                    && !string.IsNullOrEmpty(signUpVm.FirstName.Trim())
                    && !string.IsNullOrEmpty(signUpVm.LastName.Trim())
                    && !string.IsNullOrEmpty(signUpVm.UserName.Trim()))
                {
                    string imageFinalUrl;
                    if (signUpVm.ImageUrl != null)
                    {
                        var extImage = Path.GetExtension(signUpVm.ImageUrl.FileName);
                        imageFinalUrl = Builder.BuildNameForImages() + extImage;
                        var pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", imageFinalUrl);

                        await using var fileStream = new FileStream(pathImage, FileMode.Create);
                        await signUpVm.ImageUrl.CopyToAsync(fileStream);
                    }
                    else
                    {
                        imageFinalUrl = "eeeeeeeeeeeeeeee.jpg";
                    }

                    Model.Models.Main.User.User user = new Model.Models.Main.User.User()
                    {
                        Email = signUpVm.Email.Trim(),
                        Name = signUpVm.FirstName.Trim(),
                        Family = signUpVm.LastName.Trim(),
                        UserName = signUpVm.UserName.Trim(),
                        IsAccessValid = true,
                        ImageUrl = imageFinalUrl,
                        RegistryDate = DateTime.UtcNow
                    };
                    var result = await _userManager.CreateAsync(user, signUpVm.Password.Trim());
                    if (result.Succeeded)
                    {
                        var claimResult = await _userManager.AddClaimAsync(user, new Claim("UserType", "UsualUser"));
                        if (claimResult.Succeeded)
                        {
                            await _signInManager.PasswordSignInAsync(user, signUpVm.Password, true, false);

                            if (signUpVm.SubscribeMeToTheNewsletter == "yes")
                            {
                                await VerifyEmailForNewsLetter(user.Email);
                            }

                            return Redirect("/");
                        }
                        else
                        {
                            signUpVm.Message = "مشکلی پیش آمده است!";
                            return View(signUpVm);
                        }
                    }
                    else
                    {
                        signUpVm.Message = "مشکلی پیش آمده است!";
                        return View(signUpVm);
                    }
                }
                else
                {
                    signUpVm.Message = "مشکلی پیش آمده است!";
                    return View(signUpVm);
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
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
                return RedirectToAction("SignOut");
            }
            return RedirectToAction("List", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            await userActions.SendForgetPassword(email);

            return Redirect("/User/Home/List");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveForgetPasswordMessage(string key)
        {
            var currentForgetPassword = await _unitOfWork.ForgetPasswordRepository.FindAsync(fp => fp.Key == key);
            if (currentForgetPassword == null || currentForgetPassword.IsChangePasswordFinished)
            {
                return Redirect("404.cshtml");
            }
            else
            {
                ReceiveForgetPasswordMessageVm receiveForgetPasswordMessageVm = new ReceiveForgetPasswordMessageVm()
                {
                    Id = currentForgetPassword.Id,
                    Key = currentForgetPassword.Key
                };
                return View(receiveForgetPasswordMessageVm);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveForgetPasswordMessage(ReceiveForgetPasswordMessageVm receiveForgetPasswordMessageVm)
        {
            UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            await userActions.ChangeForgotPassword(receiveForgetPasswordMessageVm);

            return Redirect("/Identity/Account/Signin");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task VerifyEmailForNewsLetter(string email)
        {
            UserActions userActions = new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            await userActions.SendEmailForNewsLetter(email);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveNewsLetterVerifyEmailMessage(string key)
        {
            var currentNewsLetter = await _unitOfWork.NewsLetterRepository.FindAsync(nl => nl.Key == key);
            string finalMessage = null;
            if (currentNewsLetter == null)
            {
                return View("404");
            }
            else
            {
                if (currentNewsLetter.IsEmailVerified)
                {
                    finalMessage = ".این ایمیل قبلا تایید شده است";
                }
                else
                {
                    NewsLetter newNewsLetter = new NewsLetter()
                    {
                        CreateDate = currentNewsLetter.CreateDate,
                        Email = currentNewsLetter.Email,
                        Id = currentNewsLetter.Id,
                        IsEmailVerified = true,
                        Key = currentNewsLetter.Key
                    };
                    await _unitOfWork.NewsLetterRepository.Update(newNewsLetter);
                    await _unitOfWork.SaveAsync();
                    finalMessage = "ایمیل شما تایید شد!";
                }
                return View(model: finalMessage);
            }
        }
    }
}
