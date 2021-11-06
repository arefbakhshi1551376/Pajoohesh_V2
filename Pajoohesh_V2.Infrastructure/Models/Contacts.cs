using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ContactUs;
using Pajoohesh_V2.Utility;

namespace Pajoohesh_V2.Infrastructure.Models
{
    public class Contacts
    {
        private IUnitOfWork _unitOfWork;

        public Contacts(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ContactUsListVm> ContactListVms(int pageNumber)
        {
            int numberOfContactUsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfContactUsInEachPage;
            int numberOfContactUs = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfContactUs % numberOfContactUsInEachPage;
            var numberOfPages = (int) numberOfContactUs / numberOfContactUsInEachPage;
            if (numberOfContactUs > numberOfContactUsInEachPage)
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

            var lastContactUs =
                await _unitOfWork.ContactUsRepository.GetAllAsync(skipNumber, numberOfContactUsInEachPage);
            List<ContactUsListBaseVm> contactUsListBaseVms = new List<ContactUsListBaseVm>();

            foreach (var contactU in lastContactUs)
            {
                var currentContactUsListBaseVm = new ContactUsListBaseVm()
                {
                    Text = contactU.Text,
                    Id = contactU.Id,
                    IsReplied = contactU.IsReplied,
                    SendDate = contactU.SendDate.ToPersianDate(),
                    SenderEmail = contactU.SenderEmail,
                    SenderFamily = contactU.SenderFamily,
                    SenderName = contactU.SenderName,
                    Subject = contactU.Subject,
                    Replier = contactU.Replier.GetName(),
                    ReplyDate = contactU.ReplyDate?.ToPersianDate(),
                };
                contactUsListBaseVms.Add(currentContactUsListBaseVm);
            }

            ContactUsListVm contactUsListVm = null;


            contactUsListVm = new ContactUsListVm()
            {
                NumberOfPages = numberOfPages,
                CurrentPageNumber = pageNumber,
                ContactUsListBaseVms = contactUsListBaseVms
            };


            return contactUsListVm;
        }

        public async Task<ContactUsListVm> ContactListApiVms(string userId, int pageNumber)
        {
            int numberOfContactUsInEachPage = 30;

            int skipNumber = (pageNumber - 1) * numberOfContactUsInEachPage;
            int numberOfContactUs = await _unitOfWork.FilmRepository.GetCount();
            var reminder = numberOfContactUs % numberOfContactUsInEachPage;
            var numberOfPages = (int) numberOfContactUs / numberOfContactUsInEachPage;
            if (numberOfContactUs > numberOfContactUsInEachPage)
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

            var lastContactUs = await _unitOfWork
                .ContactUsRepository
                .GetAllAsync(skipNumber,
                    numberOfContactUsInEachPage,
                    cu => cu.SenderId == userId);
            List<ContactUsListBaseVm> contactUsListBaseVms = new List<ContactUsListBaseVm>();

            foreach (var contactUs in lastContactUs)
            {
                var currentContactUsListBaseVm = new ContactUsListBaseVm()
                {
                    Text = contactUs.Text,
                    Id = contactUs.Id,
                    IsReplied = contactUs.IsReplied,
                    SendDate = contactUs.SendDate.ToPersianDate(),
                    SenderEmail = contactUs.SenderEmail,
                    SenderFamily = contactUs.SenderFamily,
                    SenderName = contactUs.SenderName,
                    Subject = contactUs.Subject,
                    Replier = contactUs.Replier.GetName(),
                    ReplyDate = contactUs.ReplyDate?.ToPersianDate(),
                };
                contactUsListBaseVms.Add(currentContactUsListBaseVm);
            }

            ContactUsListVm contactUsListVm = null;


            contactUsListVm = new ContactUsListVm()
            {
                NumberOfPages = numberOfPages,
                CurrentPageNumber = pageNumber,
                ContactUsListBaseVms = contactUsListBaseVms
            };


            return contactUsListVm;
        }

        public async Task<ContactUsDetailsVm> ContactDetailsVms(int id)
        {
            var currentContact = await _unitOfWork.ContactUsRepository.FindAsync(c => c.Id == id);
            ContactUsDetailsVm contactUsDetailsVm = new ContactUsDetailsVm()
            {
                ReplySubject = currentContact.ReplySubject,
                ReplyText = currentContact.ReplyText,
                SendDate = currentContact.SendDate.ToPersianDate(),
                SenderEmail = currentContact.SenderEmail,
                Id = currentContact.Id,
                IsReplied = currentContact.IsReplied,
                Replier = currentContact.Replier.GetName(),
                ReplyDate = currentContact.ReplyDate?.ToPersianDate(),
                SenderFamily = currentContact.SenderFamily,
                SenderName = currentContact.SenderName,
                Subject = currentContact.Subject,
                Text = currentContact.Text
            };
            return contactUsDetailsVm;
        }

        public async Task<ContactUsReplyVm> ContactReplyVms(int id)
        {
            var currentContact = await _unitOfWork.ContactUsRepository.FindAsync(c => c.Id == id);
            ContactUsReplyVm contactUsReplyVm = new ContactUsReplyVm()
            {
                Id = currentContact.Id,
                IsReplied = false,
                OriginalSendDate = currentContact.SendDate,
                SendDate = currentContact.SendDate.ToPersianDate(),
                SenderEmail = currentContact.SenderEmail,
                SenderFamily = currentContact.SenderFamily,
                SenderName = currentContact.SenderName,
                Subject = currentContact.Subject,
                Text = currentContact.Text
            };
            return contactUsReplyVm;
        }
    }
}