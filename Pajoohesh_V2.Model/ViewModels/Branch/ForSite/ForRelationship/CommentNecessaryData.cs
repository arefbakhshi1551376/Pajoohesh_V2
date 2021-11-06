using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRelationship
{
    public class CommentNecessaryData:TotalProperties
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public FilmNecessaryData Film { get; set; }
        public State State { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string LaseModifyDate { get; set; }
    }
}
