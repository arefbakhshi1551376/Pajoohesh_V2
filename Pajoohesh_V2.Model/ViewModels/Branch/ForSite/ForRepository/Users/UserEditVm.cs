using Microsoft.AspNetCore.Http;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Users
{
    public class UserEditVm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile UserAvatar { get; set; }
        public bool IsAccessValid { get; set; }
    }
}
