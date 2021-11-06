using System.Collections.Generic;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Tags
{
    public class TagListVm
    {
        public List<TagListBaseVm> TagListBaseVms { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
