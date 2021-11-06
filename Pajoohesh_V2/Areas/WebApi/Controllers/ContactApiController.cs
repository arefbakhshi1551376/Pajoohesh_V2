using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Infrastructure.Models;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Receive.ContactUs;
using Pajoohesh_V2.Model.ViewModels.Branch.ForApplication.Send.ContactUs;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ContactUs;

namespace Pajoohesh_V2.Areas.WebApi.Controllers
{
    [Authorize]
    [Area("WebApi")]
    public class ContactApiController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private Contacts contactsModel;

        public ContactApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> List(SendContactUsList sendContactUsList)
        {
            contactsModel = new Contacts(_unitOfWork);
            ReceiveContactUsList receiveContactUsList = new ReceiveContactUsList();
            ContactUsListVm contactUsListVm = await contactsModel.ContactListApiVms(sendContactUsList.UserId,sendContactUsList.PageNumber);
            
            receiveContactUsList.ContactUsListBaseVms = contactUsListVm.ContactUsListBaseVms;
            receiveContactUsList.CurrentPageNumber = contactUsListVm.CurrentPageNumber;
            receiveContactUsList.NumberOfPages = contactUsListVm.NumberOfPages;
            
            return Json(receiveContactUsList);
        }
    }
}