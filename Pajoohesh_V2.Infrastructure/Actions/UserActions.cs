using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ForgetPasswords;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Users;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Actions
{
    public class UserActions
    {
        private UserManager<User> _userManager;
        private IWebHostEnvironment env;
        public IUserRepository _userRepository;
        private IUnitOfWork _unitOfWork;
        private IHttpContextAccessor _httpContextAccessor;

        public UserActions(UserManager<User> userManager, IWebHostEnvironment env, IUserRepository userRepository,
            IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            this.env = env;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task UpdateAsync(UserEditVm userEditVm)
        {
            var currentUser = await _userManager.FindByIdAsync(userEditVm.Id);
            if (userEditVm.UserAvatar != null ||
                currentUser.UserName != userEditVm.UserName.Trim() ||
                currentUser.Name != userEditVm.Name.Trim() ||
                currentUser.Family != userEditVm.Family.Trim() ||
                currentUser.IsAccessValid != userEditVm.IsAccessValid ||
                (!string.IsNullOrEmpty(userEditVm.NewPassword) && !string.IsNullOrEmpty(userEditVm.ConfirmPassword) &&
                 userEditVm.NewPassword == userEditVm.ConfirmPassword))
            {
                string imageFinalUrl = null;
                string finalPasswordHash = null;

                if (!string.IsNullOrEmpty(userEditVm.NewPassword) &&
                    !string.IsNullOrEmpty(userEditVm.ConfirmPassword) &&
                    userEditVm.NewPassword == userEditVm.ConfirmPassword)
                {
                    var newPassword =
                        _userManager.PasswordHasher.HashPassword(currentUser, userEditVm.NewPassword.Trim());
                    finalPasswordHash = newPassword;
                }
                else if (string.IsNullOrEmpty(userEditVm.NewPassword) ||
                         string.IsNullOrEmpty(userEditVm.ConfirmPassword))
                {
                    finalPasswordHash = currentUser.PasswordHash;
                }

                if (userEditVm.UserAvatar != null)
                {
                    if (userEditVm.ImageUrl != null)
                    {
                        var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", userEditVm.ImageUrl);
                        File.Delete(oldPathImage);
                    }

                    var extImage = Path.GetExtension(userEditVm.UserAvatar.FileName);
                    imageFinalUrl = Builder.BuildNameForImages() + extImage;
                    var pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", imageFinalUrl);

                    await using var fileStream = new FileStream(pathImage, FileMode.Create);
                    await userEditVm.UserAvatar.CopyToAsync(fileStream);
                }
                else
                {
                    imageFinalUrl = userEditVm.ImageUrl ?? "eeeeeeeeeeeeeeee.jpg";
                }

                currentUser.IsAccessValid = userEditVm.IsAccessValid;
                currentUser.ImageUrl = imageFinalUrl;
                currentUser.Email = userEditVm.Email;
                currentUser.Id = userEditVm.Id;
                currentUser.UserName = userEditVm.UserName;
                currentUser.Name = userEditVm.Name;
                currentUser.Family = userEditVm.Family;
                currentUser.PasswordHash = finalPasswordHash;
                currentUser.RegistryDate = currentUser.RegistryDate;

                await _userRepository.UpdateAsync(currentUser);
            }
        }

        public async Task AddAsync(UserAddVm userAddVm)
        {
            var currentUser = await _userManager.FindByEmailAsync(userAddVm.Email.Trim());
            if (currentUser == null)
            {
                if (!string.IsNullOrEmpty(userAddVm.Password.Trim())
                    && !string.IsNullOrEmpty(userAddVm.ConfirmPassword.Trim())
                    && userAddVm.Password.Trim().Equals(userAddVm.ConfirmPassword.Trim())
                    && !string.IsNullOrEmpty(userAddVm.Email.Trim())
                    && !string.IsNullOrEmpty(userAddVm.Name.Trim())
                    && !string.IsNullOrEmpty(userAddVm.Family.Trim())
                    && !string.IsNullOrEmpty(userAddVm.UserName.Trim()))
                {
                    string imageFinalUrl = null;
                    if (userAddVm.ImageUrl != null)
                    {
                        var extImage = Path.GetExtension(userAddVm.ImageUrl.FileName);
                        imageFinalUrl = Builder.BuildNameForImages() + extImage;
                        var pathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", imageFinalUrl);

                        await using var fileStream = new FileStream(pathImage, FileMode.Create);
                        await userAddVm.ImageUrl.CopyToAsync(fileStream);
                    }
                    else
                    {
                        imageFinalUrl = "eeeeeeeeeeeeeeee.jpg";
                    }

                    User user = new User()
                    {
                        Email = userAddVm.Email.Trim(),
                        Name = userAddVm.Name.Trim(),
                        Family = userAddVm.Family.Trim(),
                        UserName = userAddVm.UserName.Trim(),
                        IsAccessValid = true,
                        ImageUrl = imageFinalUrl,
                        RegistryDate = DateTime.UtcNow
                    };

                    var isUserAdded = await _userRepository.AddAsync(user, userAddVm.Password);
                    if (!isUserAdded)
                    {
                        var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", user.ImageUrl);
                        System.IO.File.Delete(oldPathImage);
                    }

                    if (userAddVm.SubscribeMeToTheNewsletter == "yes")
                    {
                        await SendEmailForNewsLetter(userAddVm.Email);
                    }
                }
            }
        }

        public async Task ChangeForgotPassword(ReceiveForgetPasswordMessageVm receiveForgetPasswordMessageVm)
        {
            string finalPasswordHash = null;

            var currentForgetPassword = await _unitOfWork
                .ForgetPasswordRepository.FindAsync(fp => fp.Id == receiveForgetPasswordMessageVm.Id);
            var currentUser = currentForgetPassword.User;

            if (!string.IsNullOrEmpty(receiveForgetPasswordMessageVm.NewPassword)
                && !string.IsNullOrEmpty(receiveForgetPasswordMessageVm.ConfirmPassword)
                && receiveForgetPasswordMessageVm.NewPassword == receiveForgetPasswordMessageVm.ConfirmPassword)
            {
                var newPassword = _userManager.PasswordHasher.HashPassword(currentUser,
                    receiveForgetPasswordMessageVm.NewPassword.Trim());
                finalPasswordHash = newPassword;
            }

            currentUser.PasswordHash = finalPasswordHash;

            await _userRepository.UpdateAsync(currentUser);

            ForgetPassword forgetPassword = new ForgetPassword()
            {
                User = currentUser,
                IsChangePasswordFinished = true,
                CreateDate = currentForgetPassword.CreateDate,
                Id = currentForgetPassword.Id,
                Key = currentForgetPassword.Key
            };

            await _unitOfWork.ForgetPasswordRepository.Update(forgetPassword);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(User user)
        {
            await _userRepository.DeleteAsync(user);

            if (user.ImageUrl != null)
            {
                var oldPathImage = Path.Combine(env.WebRootPath + "\\Media\\Images\\User", user.ImageUrl);
                System.IO.File.Delete(oldPathImage);
            }
        }

        public async Task SendForgetPassword(string email)
        {
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser != null)
            {
                var key = Builder.BuildKeyForForgetPassword();
                string currentDomain = _httpContextAccessor.HttpContext.Request.Host.Value;
                SendMessages sendMessages = new SendMessages();
                string subject = "تغییر رمز عبور!";
                string message =
                    "درخواستی مبنی بر تغییر رمز عبور از طرف شما ارسال شده است. برای تغییر رمز عبور خود روی لینک زیر کلیک کنید:\n" +
                    $"https://{currentDomain}/Identity/Account/ReceiveForgetPasswordMessage?key={key}";
                await sendMessages.SendEmailAsync(email, subject, message);

                Model.Models.Main.User.ForgetPassword forgetPassword = new ForgetPassword()
                {
                    CreateDate = DateTime.UtcNow,
                    Key = key,
                    User = currentUser,
                    IsChangePasswordFinished = false
                };

                await _unitOfWork.ForgetPasswordRepository.AddAsync(forgetPassword);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task SendEmailForNewsLetter(string email)
        {
            var currentNewsLetter = await _unitOfWork.NewsLetterRepository.FindAsync(nl => nl.Email == email);

            if (currentNewsLetter == null)
            {
                var key = Builder.BuildKeyForNewsLetter();
                string currentDomain = _httpContextAccessor.HttpContext.Request.Host.Value;
                SendMessages sendMessages = new SendMessages();
                string subject = "تایید ایمیل برای خبرنامه";
                string message =
                    "درخواستی مبنی بر تمایل به دریافت آخرین اخبار سایت از طرف شما ارسال شده است. برای این منظور روی لینک زیر کلیک کنید:\n" +
                    $"https://{currentDomain}/Identity/Account/ReceiveNewsLetterVerifyEmailMessage?key={key}";
                await sendMessages.SendEmailAsync(email, subject, message);

                NewsLetter newNewsLetter = new NewsLetter()
                {
                    Email = email,
                    IsEmailVerified = false,
                    Key = key,
                    CreateDate = DateTime.UtcNow
                };

                await _unitOfWork.NewsLetterRepository.AddAsync(newNewsLetter);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}