using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Tag;

namespace Pajoohesh_V2.Model.Models.Relationship
{
    public class FilmTag
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public int FilmId { get; set; }
        public Film Film { get; set; }
    }
}
