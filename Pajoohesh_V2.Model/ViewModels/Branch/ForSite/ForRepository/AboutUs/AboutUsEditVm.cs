using System;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.AboutUs
{
    public class AboutUsEditVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string CreatorId { get; set; }
        public string CreateDate { get; set; }
        public DateTime OriginalCreateDate { get; set; }
    }
}
