using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Pajoohesh_V2.Model.Models.Relationship;

namespace Pajoohesh_V2.Model.Models.Main.User
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public bool IsAccessValid { get; set; }
        public bool IsEmailVerified { get; set; }
        public string MobilePhoneNumber { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string ImageUrl { get; set; }
        public DateTime RegistryDate { get; set; }
        public List<FilmLikeUser> LikeUsers { get; set; }
    }
}
