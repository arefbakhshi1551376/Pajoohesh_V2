using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.Models.Main.Subject;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmAddVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public Subject Subject { get; set; }
        public int SubjectId { get; set; }
        public State State { get; set; }
        public SelectList SubjectSelectList { get; set; }
        public IFormFile ImageUrl { get; set; }
        public List<IFormFile> FilmUrl { get; set; }
    }
}