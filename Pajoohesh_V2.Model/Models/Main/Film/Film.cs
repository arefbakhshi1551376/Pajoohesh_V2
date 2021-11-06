using System;
using System.Collections.Generic;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Relationship;

namespace Pajoohesh_V2.Model.Models.Main.Film
{
    public class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilmUrl { get; set; }
        public string ImageUrl { get; set; }
        public List<FilmTag> FilmTags { get; set; }
        public List<Comment.Comment> Comments { get; set; }
        public Subject.Subject Subject { get; set; }
        public State State { get; set; }
        public List<FilmLikeUser> LikeUsers { get; set; }
        public int NumberOfViews { get; set; }
        public User.User Uploader { get; set; }
        public DateTime UploadDate { get; set; }
        public User.User LastModifier { get; set; }
        public DateTime? LaseModifyDate { get; set; }
    }
}
