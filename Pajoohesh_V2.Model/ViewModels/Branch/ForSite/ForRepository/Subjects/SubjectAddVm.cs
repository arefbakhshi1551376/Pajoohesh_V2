using Microsoft.AspNetCore.Http;
using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Subjects
{
    public class SubjectAddVm
    {
        public string Title { get; set; }
        public State State { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}