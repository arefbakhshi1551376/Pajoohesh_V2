using System.Collections.Generic;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Main;
using Pajoohesh_V2.Model.Models.Relationship;

namespace Pajoohesh_V2.Data.Repoditory.IData.Branch.Other
{
    public interface IFilmTagRepository : IRepository<FilmTag>
    {
        Task<FilmTag> FindByFilmIdAsync(int id);
        Task<FilmTag> FindByTagIdAsync(int id);
    }
}
