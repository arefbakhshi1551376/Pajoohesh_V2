using System;
using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmListBaseVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public State State { get; set; }
        public string ImageUrl { get; set; }
        public int NumberOfViews { get; set; }
        public string UploaderId { get; set; }
        public string Uploader { get; set; }
        public string UploaderImage { get; set; }
        public string UploadDate { get; set; }
        public DateTime OriginalUploadDate { get; set; }
        public string UploadPastDateTime { get; set; }
    }
}
