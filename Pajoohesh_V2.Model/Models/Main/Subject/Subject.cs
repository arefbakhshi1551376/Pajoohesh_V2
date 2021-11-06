using System;
using System.Collections.Generic;
using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.Models.Main.Subject
{
    public class Subject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Film.Film> Films { get; set; }
        public State State { get; set; }
        public string ImageUrl { get; set; }
        public User.User Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public User.User LastModifier { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
