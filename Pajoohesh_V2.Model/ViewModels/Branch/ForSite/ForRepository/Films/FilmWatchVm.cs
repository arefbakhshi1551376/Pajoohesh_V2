using System;
using System.Collections.Generic;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Comments;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmWatchVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string FilmUrl { get; set; }
        public Subject Subject { get; set; }
        public List<Tag> Tags { get; set; }
        public int NumberOfViews { get; set; }
        public int NumberOfLikes { get; set; }
        public CommentListVm CommentListVm { get; set; }
        public string Uploader { get; set; }
        public string UploaderImage { get; set; }
        public string UploadDate { get; set; }
        public DateTime OriginalUploadDate { get; set; }
        public string UploadPastDateTime { get; set; }
        public List<RelatedFilmsVm> RelatedFilmsVms { get; set; }
    }
}
