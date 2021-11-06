using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Comment;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmEditVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilmUrl { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile NewImageUrl { get; set; }
        public List<IFormFile> NewFilmUrl { get; set; }
        public string Tags { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Subject> Subjects { get; set; }
        public int SubjectId { get; set; }
        public List<User> LikeUsers { get; set; }
        public int NumberOfViews { get; set; }
        public State State { get; set; }
        public string UploaderId { get; set; }
        public string UploadDate { get; set; }
        public DateTime OriginalUploadDate { get; set; }
        public string LastModifierId { get; set; }
        public string LastModifyDate { get; set; }
        public DateTime? OriginalLastModifyDate { get; set; }
    }
}