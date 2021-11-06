using System.Collections.Generic;
using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRelationship
{
    public class UserNecessaryData:TotalProperties
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public bool IsAccessValid { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string MobilePhoneNumber { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAdmin { get; set; }
        public string RegistryDate { get; set; }
    }
}
