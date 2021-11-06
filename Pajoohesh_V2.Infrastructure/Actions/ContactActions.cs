using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.ContactUs;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ContactUs;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Actions
{
    public class ContactActions
    {
        private UserManager<Model.Models.Main.User.User> _userManager;
        private SignInManager<Model.Models.Main.User.User> _signInManager;
        private IUnitOfWork _unitOfWork;

        public ContactActions(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public async Task ContactReplyAction(ContactUsReplyVm contactUsReplyVm, User user)
        {
            var currentContact = await _unitOfWork.ContactUsRepository.FindAsync(c => c.Id == contactUsReplyVm.Id);
            SendMessages sendMessages = new SendMessages();
            await sendMessages.SendEmailAsync(currentContact.SenderEmail, contactUsReplyVm.ReplySubject,
                contactUsReplyVm.ReplyText);

            ContactUs currentContactUs = new ContactUs()
            {
                SenderFamily = currentContact.SenderFamily,
                SenderName = currentContact.SenderName,
                Subject = currentContact.Subject,
                Text = currentContact.Text,
                Id = currentContact.Id,
                IsReplied = true,
                Replier = user,
                ReplyDate = DateTime.UtcNow,
                SendDate = currentContact.SendDate,
                SenderEmail = currentContact.SenderEmail,
                ReplySubject = contactUsReplyVm.ReplySubject,
                ReplyText = contactUsReplyVm.ReplyText
            };
            await _unitOfWork.ContactUsRepository.Update(currentContactUs);
            await _unitOfWork.SaveAsync();
        }

        public async Task ContactAddAction(ContactUsAddVm contactUsAddVm)
        {
            var contactUs = new ContactUs()
            {
                IsReplied = false,
                Replier = null,
                ReplyDate = null,
                SendDate = DateTime.UtcNow,
                SenderEmail = contactUsAddVm.SenderEmail,
                SenderFamily = contactUsAddVm.SenderFamily,
                SenderName = contactUsAddVm.SenderName,
                Subject = contactUsAddVm.Subject,
                Text = contactUsAddVm.Text,
                SenderId = contactUsAddVm.SenderId
            };
            await _unitOfWork.ContactUsRepository.AddAsync(contactUs);
            await _unitOfWork.SaveAsync();
        }
    }
}