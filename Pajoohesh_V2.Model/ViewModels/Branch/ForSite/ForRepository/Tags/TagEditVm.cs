using System;
using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Tags
{
    public class TagEditVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public State State { get; set; }
        public string CreatorId { get; set; }
        public string CreateDate { get; set; }
        public DateTime OriginalCreateDate { get; set; }
        public string LastModifierId { get; set; }
        public string LastModifyDate { get; set; }
        public DateTime? OriginalLastModifyDate { get; set; }
    }
}