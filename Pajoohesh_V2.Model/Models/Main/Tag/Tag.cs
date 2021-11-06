using System;
using System.Collections.Generic;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Relationship;

namespace Pajoohesh_V2.Model.Models.Main.Tag
{
    public class Tag
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<FilmTag> FilmTags { get; set; }
        public State State { get; set; }
        public User.User Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public User.User LastModifier { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
