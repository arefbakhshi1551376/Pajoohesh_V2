using System;
using System.Collections.Generic;
using System.Text;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmListApiVm
    {
        public List<FilmListBaseApiVm> FilmListBaseApiVm { get; set; }
        public int CurrentPageNumber { get; set; }
        public int NumberOfPages { get; set; }
    }
}
