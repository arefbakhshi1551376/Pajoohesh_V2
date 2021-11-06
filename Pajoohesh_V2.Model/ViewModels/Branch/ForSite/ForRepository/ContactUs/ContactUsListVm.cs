using System.Collections.Generic;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.ContactUs
{
    public class ContactUsListVm
    {
        public List<ContactUsListBaseVm> ContactUsListBaseVms { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
