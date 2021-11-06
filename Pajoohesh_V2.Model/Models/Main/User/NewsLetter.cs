using System;
using System.Collections.Generic;
using System.Text;

namespace Pajoohesh_V2.Model.Models.Main.User
{
    public class NewsLetter
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Key { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
