using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects
{
    public class SubjectDeleteVm:TotalProperties
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int NumberOfFilms { get; set; }
        public string ImageUrl { get; set; }
    }
}
