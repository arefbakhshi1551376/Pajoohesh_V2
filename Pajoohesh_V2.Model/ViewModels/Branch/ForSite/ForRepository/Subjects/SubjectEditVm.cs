using System;
using Microsoft.AspNetCore.Http;
using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects
{
    public class SubjectEditVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public State State { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile NewImageUrl { get; set; }
        public string CreatorId { get; set; }
        public string CreateDate { get; set; }
        public DateTime OriginalCreateDate { get; set; }
        public string LastModifierId { get; set; }
        public string LastModifyDate { get; set; }
        public DateTime? OriginalLastModifyDate { get; set; }
    }
}