using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Pajoohesh_V2.Data.Initializer;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Actions;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.Account;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.Account;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.User;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Areas.WebApi.Controllers
{
    [Authorize]
    [Area("WebApi")]
    public class AccountApiController : Controller
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;
        private IWebHostEnvironment env;
        private IHttpContextAccessor _httpContextAccessor;

        public AccountApiController(UserManager<Model.Models.Main.User.User> userManager,
            SignInManager<Model.Models.Main.User.User> signInManager, IUnitOfWork unitOfWork,
            IUserRepository userRepository, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            this.env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin([FromBody]SendAccountSignIn sendAccountSignIn)
        {
            ReceiveAccountSignIn receiveAccountSignIn = new ReceiveAccountSignIn()
            {
                PassWord = sendAccountSignIn.PassWord,
                RememberMe = sendAccountSignIn.RememberMe,
                UserName = sendAccountSignIn.UserName
            };
            if (!string.IsNullOrEmpty(sendAccountSignIn.UserName)
                && !string.IsNullOrEmpty(sendAccountSignIn.PassWord))
            {
                var userByName = await _userManager.FindByNameAsync(sendAccountSignIn.UserName);
                var userByEmail = await _userManager.FindByEmailAsync(sendAccountSignIn.UserName);
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
                        var result = await _signInManager.PasswordSignInAsync(finalUser, sendAccountSignIn.PassWord,
                            sendAccountSignIn.RememberMe, false);
                        if (result.Succeeded)
                        {
                            var claimsCheck = await _userManager.GetClaimsAsync(finalUser);
                            var isAdmin = await _userManager.IsInRoleAsync(finalUser, Constants.AdminRole);
                            if (isAdmin)
                            {
                                receiveAccountSignIn.Message = "خوش آمدید! شما مدیر هستید!";
                                var token = await this.Token(receiveAccountSignIn);
                                return Json(token);
                            }
                            else
                            {
                                if (claimsCheck.Any(c => c.Value == Constants.AdminClaim))
                                {
                                    receiveAccountSignIn.Message = "خوش آمدید!";
                                    var token = await this.Token(receiveAccountSignIn);
                                    return Json(token);
                                }
                                else
                                {
                                    if (claimsCheck.Any(c => c.Value == Constants.UserClaim))
                                    {
                                        receiveAccountSignIn.Message = "خوش آمدید!";
                                        var token = await this.Token(receiveAccountSignIn);
                                        return Json(token);
                                    }
                                    else
                                    {
                                        receiveAccountSignIn.Message = "مشکلی پیش آمده است!";
                                        return Json(receiveAccountSignIn);
                                    }
                                }
                            }
                        }
                        else
                        {
                            receiveAccountSignIn.Message = "مشکلی پیش آمده است!";
                            return Json(receiveAccountSignIn);
                        }
                    }
                    else
                    {
                        receiveAccountSignIn.Message = "دسترسی شما به سایت مجاز نیست!";
                        return Json(receiveAccountSignIn);
                    }
                }
                else
                {
                    receiveAccountSignIn.Message = "چنین کاربری وجود ندارد!";
                    return Json(receiveAccountSignIn);
                }
            }
            else
            {
                receiveAccountSignIn.Message = "لطفا اطلاعات خود را وارد نمایید!";
                return Json(receiveAccountSignIn);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SendAccountSignUp sendAccountSignUp)
        {
            ReceiveAccountSignUp receiveAccountSignUp = new ReceiveAccountSignUp()
            {
                Email = sendAccountSignUp.Email,
                Password = sendAccountSignUp.Password,
                ConfirmPassword = sendAccountSignUp.ConfirmPassword,
                FirstName = sendAccountSignUp.FirstName,
                ImageUrl = sendAccountSignUp.ImageUrl,
                LastName = sendAccountSignUp.LastName,
                UserName = sendAccountSignUp.UserName,
                SubscribeMeToTheNewsletter = sendAccountSignUp.SubscribeMeToTheNewsletter
            };
            var userEmails = _userManager.Users.Select(u => u.Email);
            var userNames = _userManager.Users.Select(u => u.UserName);
            if (userEmails.Contains(sendAccountSignUp.Email) || userNames.Contains(sendAccountSignUp.UserName))
            {
                sendAccountSignUp.Message = "کابری با این ایمیل یا نام کاربری وجود دارد!";
                return Json(sendAccountSignUp);
            }
            else
            {
                if (!string.IsNullOrEmpty(sendAccountSignUp.Password.Trim())
                    && !string.IsNullOrEmpty(sendAccountSignUp.ConfirmPassword.Trim())
                    && sendAccountSignUp.Password.Trim().Equals(sendAccountSignUp.ConfirmPassword.Trim())
                    && !string.IsNullOrEmpty(sendAccountSignUp.Email.Trim())
                    && !string.IsNullOrEmpty(sendAccountSignUp.FirstName.Trim())
                    && !string.IsNullOrEmpty(sendAccountSignUp.LastName.Trim())
                    && !string.IsNullOrEmpty(sendAccountSignUp.UserName.Trim()))
                {
                    string imageFinalUrl;
                    if (sendAccountSignUp.ImageUrl != null)
                    {
                        var extImage = Path.GetExtension(sendAccountSignUp.ImageUrl.FileName);
                        imageFinalUrl = Builder.BuildNameForImages() + extImage;
                        var pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", imageFinalUrl);

                        await using var fileStream = new FileStream(pathImage, FileMode.Create);
                        await sendAccountSignUp.ImageUrl.CopyToAsync(fileStream);
                    }
                    else
                    {
                        imageFinalUrl = "eeeeeeeeeeeeeeee.jpg";
                    }

                    Model.Models.Main.User.User user = new Model.Models.Main.User.User()
                    {
                        Email = sendAccountSignUp.Email.Trim(),
                        Name = sendAccountSignUp.FirstName.Trim(),
                        Family = sendAccountSignUp.LastName.Trim(),
                        UserName = sendAccountSignUp.UserName.Trim(),
                        IsAccessValid = true,
                        RegistryDate = DateTime.UtcNow,
                        ImageUrl = imageFinalUrl
                    };
                    var result = await _userManager.CreateAsync(user, sendAccountSignUp.Password.Trim());
                    if (result.Succeeded)
                    {
                        var claimResult = await _userManager.AddClaimAsync(user, new Claim("UserType", "UsualUser"));
                        if (claimResult.Succeeded)
                        {
                            var signUpResult =
                                await _signInManager.PasswordSignInAsync(user, sendAccountSignUp.Password, true, false);
                            if (signUpResult.Succeeded)
                            {
                                if (sendAccountSignUp.SubscribeMeToTheNewsletter == "yes")
                                {
                                    UserActions userActions = new UserActions(_userManager, env, _userRepository,
                                        _unitOfWork, _httpContextAccessor);
                                    await userActions.SendEmailForNewsLetter(user.Email);
                                }

                                var finalPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User",
                                    user.ImageUrl);
                                receiveAccountSignUp.Message = "شما با موفقیت ثبت نام شدید!";
                                receiveAccountSignUp.FullImageUrl = finalPathImage;
                                return Json(receiveAccountSignUp);
                            }
                            else
                            {
                                receiveAccountSignUp.Message = "مشکلی در ثبت نام شما به وجود آمد!";
                                return Json(receiveAccountSignUp);
                            }
                        }
                        else
                        {
                            receiveAccountSignUp.Message = "مشکلی در ثبت نام شما به وجود آمد!";
                            return Json(receiveAccountSignUp);
                        }
                    }
                    else
                    {
                        receiveAccountSignUp.Message = "مشکلی در ثبت نام شما به وجود آمد!";
                        return Json(receiveAccountSignUp);
                    }
                }
                else
                {
                    receiveAccountSignUp.Message = "مشکلی در ثبت نام شما به وجود آمد!";
                    return Json(receiveAccountSignUp);
                }
            }
        }

        private async Task<IActionResult> Token(ReceiveAccountSignIn receiveAccountSignIn)
        {
            var userByName = await _userManager.FindByNameAsync(receiveAccountSignIn.UserName);
            var userByEmail = await _userManager.FindByEmailAsync(receiveAccountSignIn.UserName);
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

            var result = await _userManager.CheckPasswordAsync(finalUser, receiveAccountSignIn.PassWord);
            if (result)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtIdentity.Key));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var keyEncrypt = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtIdentity.Encrypt));
                var cred = new EncryptingCredentials(keyEncrypt, SecurityAlgorithms.Aes128KW,
                    SecurityAlgorithms.Aes128CbcHmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, finalUser.Id),
                    new Claim("emailInfo", finalUser.Email)
                };
                var token = new SecurityTokenDescriptor()
                {
                    Audience = JwtIdentity.Audience,
                    Issuer = JwtIdentity.Issuer,
                    SigningCredentials = credentials,
                    EncryptingCredentials = cred,
                    Expires = DateTime.UtcNow.AddDays(15),
                    Subject = new ClaimsIdentity(claims)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(token);
                var tokenResult = new
                {
                    token = tokenHandler.WriteToken(securityToken),
                    expiration = securityToken.ValidTo,
                    message = receiveAccountSignIn.Message
                };
                return Json(tokenResult);
            }
            else
            {
                receiveAccountSignIn.Message = "خطا در ورود به برنامه";
                return Json(receiveAccountSignIn);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(SendUserEditGet sendUserEditGet)
        {
            var currentUser = await _userRepository.FindAsync(sendUserEditGet.Id);
            var hostUrl = Request.Host.Value.ToString();
            //var finalPathImageForCurrentUser = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", currentUser.ImageUrl);
            var finalPathImageForCurrentUser = $"{hostUrl}/media/Images/Subject/{currentUser.ImageUrl}";
            ReceiveUserEditGet receiveUserEditGet = new ReceiveUserEditGet()
            {
                Email = currentUser.Email,
                Family = currentUser.Family,
                Id = currentUser.Id,
                Name = currentUser.Name,
                UserName = currentUser.UserName,
                IsAccessValid = currentUser.IsAccessValid,
                ImageUrl = finalPathImageForCurrentUser,
            };
            return Json(receiveUserEditGet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SendAccountEdit sendAccountEdit)
        {
            UserActions userActions =
                new UserActions(_userManager, env, _userRepository, _unitOfWork, _httpContextAccessor);
            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            await userActions.UpdateAsync(sendAccountEdit);

            var currentUser = await _userManager.FindByIdAsync(sendAccountEdit.Id);
            //var finalPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", currentUser.ImageUrl);
            var hostUrl = Request.Host.Value.ToString();
            var finalPathImage = $"{hostUrl}/media/Images/User/{currentUser.ImageUrl}";
            ReceiveAccountEdit receiveAccountEdit = new ReceiveAccountEdit()
            {
                Email = currentUser.Email,
                Family = currentUser.Family,
                Id = currentUser.Id,
                ImageUrl = finalPathImage,
                IsAccessValid = currentUser.IsAccessValid,
                Name = currentUser.Name,
                UserName = currentUser.UserName,
            };

            return Json(receiveAccountEdit);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetData(SendAccountGetData sendAccountGetData)
        {
            ReceiveAccountGetData receiveAccountGetData;
            if (User.Identity.IsAuthenticated)
            {
                var customer = await _userManager.FindByNameAsync(User.Identity.Name);
                var isAdmin = await _userManager.IsInRoleAsync(customer, "Admin");
                receiveAccountGetData = new ReceiveAccountGetData()
                {
                    Email = customer.Email,
                    Name = customer.Name,
                    Id = customer.Id,
                    Family = customer.Family,
                    IsAdmin = isAdmin,
                    ImageUrl = customer.ImageUrl,
                    RegistryDate = customer.RegistryDate.ToPersianDate(),
                    IsAccessValid = customer.IsAccessValid,
                    IsEmailVerified = customer.IsEmailVerified,
                    MobilePhoneNumber = customer.MobilePhoneNumber,
                    IsPhoneNumberVerified = customer.IsPhoneNumberVerified
                };
            }
            else
            {
                receiveAccountGetData = new ReceiveAccountGetData()
                {
                    Message = "ورود شما مجاز نیست!"
                };
            }

            return Json(receiveAccountGetData);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SignOut(SendAccountSignOut sendAccountSignOut)
        {
            ReceiveAccountSignOut receiveAccountSignOut = new ReceiveAccountSignOut();
            try
            {
                await _signInManager.SignOutAsync();
                receiveAccountSignOut.IsSignedOut = true;
            }
            catch (Exception e)
            {
                receiveAccountSignOut.IsSignedOut = false;
            }

            return Json(receiveAccountSignOut);
        }
    }
}