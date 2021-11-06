using System.Collections.Generic;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.Tag;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmDeleteVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilmUrl { get; set; }
        public string UploadDate { get; set; }
        public string UploadPastDate { get; set; }
        public List<Tag> Tags { get; set; }
        public Subject Subject { get; set; }
    }
}
