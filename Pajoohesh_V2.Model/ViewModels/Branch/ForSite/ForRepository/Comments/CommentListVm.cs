using System.Collections.Generic;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Comments
{
    public class CommentListVm
    {
        public List<CommentListBaseVm> CommentListBaseVms { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
