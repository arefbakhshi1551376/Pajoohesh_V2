using System;
using System.Collections.Generic;
using System.Text;

namespace Pajoohesh_V2.Model.Models.Main.AboutUs
{
    public class AboutUs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public User.User Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public User.User LastModifier { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
