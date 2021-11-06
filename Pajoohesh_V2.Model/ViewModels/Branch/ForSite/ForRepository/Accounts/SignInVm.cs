using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Accounts
{
    public class SignInVm:TotalProperties
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool RememberMe { get; set; }
    }
}
