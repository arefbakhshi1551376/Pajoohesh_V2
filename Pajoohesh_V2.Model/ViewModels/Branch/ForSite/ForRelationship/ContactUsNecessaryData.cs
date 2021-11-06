using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRelationship
{
    public class ContactUsNecessaryData:TotalProperties
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public bool IsReplied { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string LastModifier { get; set; }
        public string LastModifyDate { get; set; }
    }
}
