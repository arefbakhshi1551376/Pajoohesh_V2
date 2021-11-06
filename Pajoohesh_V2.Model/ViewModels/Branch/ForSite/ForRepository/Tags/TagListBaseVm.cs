using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Tags
{
    public class TagListBaseVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public State State { get; set; }
        public int NumberOfFilms { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string LastModifier { get; set; }
        public string LastModifyDate { get; set; }
    }
}