using Microsoft.AspNetCore.Http;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Users
{
    public class UserAddVm
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SubscribeMeToTheNewsletter { get; set; }
    }
}
