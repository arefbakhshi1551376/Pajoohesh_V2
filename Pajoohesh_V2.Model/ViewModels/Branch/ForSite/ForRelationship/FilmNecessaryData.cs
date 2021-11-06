using System.Collections.Generic;
using Pajoohesh_V2.Model.Main;
using Pajoohesh_V2.Model.ViewModels.Trunk;

namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRelationship
{
    public class FilmNecessaryData:TotalProperties
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilmUrl { get; set; }
        public string ImageUrl { get; set; }
        public List<TagNecessaryData> Tags { get; set; }
        public List<CommentNecessaryData> Comments { get; set; }
        public int SubjectId { get; set; }
        public string SubjectTitle { get; set; }
        public State State { get; set; }
        public List<UserNecessaryData> LikeUsers { get; set; }
        public int NumberOfViews { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string LastModifier { get; set; }
        public string LastModifyDate { get; set; }
    }
}
