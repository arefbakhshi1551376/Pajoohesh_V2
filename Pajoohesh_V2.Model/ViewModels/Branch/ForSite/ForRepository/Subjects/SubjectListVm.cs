using System.Collections.Generic;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects
{
    public class SubjectListVm
    {
        public List<SubjectListBaseVm> SubjectListBaseVms { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
