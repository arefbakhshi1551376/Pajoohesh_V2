using System.Collections.Generic;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmListVm
    {
        public List<FilmListBaseVm> FilmListBaseVms { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
