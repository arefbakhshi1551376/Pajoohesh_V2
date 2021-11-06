using Pajoohesh_V2.Model.Main;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Comments
{
    public class CommentListBaseVm
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public State State { get; set; }
        public string Creator { get; set; }
        public string CreatorImage { get; set; }
        public string CreateDate { get; set; }
        public string PastCreateDate { get; set; }
        public string LaseModifyDate { get; set; }
        public string PastLaseModifyDate { get; set; }
    }
}
