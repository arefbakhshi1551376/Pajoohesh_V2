namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Users
{
    public class UserListBaseVm
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAccessValid { get; set; }
        public bool IsAdmin { get; set; }
        public string RegisterDate { get; set; }
        public string ImageUrl { get; set; }
    }
}
