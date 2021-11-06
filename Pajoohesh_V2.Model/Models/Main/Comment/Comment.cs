using System;
using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.Models.Main.Comment
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Film.Film Film { get; set; }
        public State State { get; set; }
        public User.User Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LaseModifyDate { get; set; }
    }
}
