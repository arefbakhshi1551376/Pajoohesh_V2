using Microsoft.AspNetCore.Http;
using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Accounts
{
    public class SignUpVm : TotalProperties
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string SubscribeMeToTheNewsletter { get; set; }
    }
}