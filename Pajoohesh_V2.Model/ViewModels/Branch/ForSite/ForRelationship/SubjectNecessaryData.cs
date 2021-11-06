using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRelationship
{
    public class SubjectNecessaryData:TotalProperties
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public State State { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string LastModifier { get; set; }
        public string LastModifyDate { get; set; }
    }
}
