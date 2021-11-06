namespace Pajoohesh_V2.Model.ViewModels.Branch.ForSite.ForRepository.Films
{
    public class FilmLikeDislikeVm
    {
        public int FilmId { get; set; }
        public string UserId { get; set; }
        public bool IsLikedByThisUser { get; set; }
        public int NumberOfLikes { get; set; }
    }
}
