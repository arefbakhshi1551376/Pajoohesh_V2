using System;
using System.Collections.Generic;
using System.Text;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Model.Models.Relationship
{
    public class FilmLikeUser
    {
        public int FilmId { get; set; }
        public Film Film { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
