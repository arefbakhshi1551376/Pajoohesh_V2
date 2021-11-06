using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Main;
using Pajoohesh_V2.Model.Models.Main.Film;

namespace Pajoohesh_V2.Data.Repoditory.IData.Branch.Other
{
    public interface IFilmRepository : IRepository<Film>
    {
        Task<IEnumerable<Film>> GetAllAsync(int skipNumber,
            int takeNumber, Expression<Func<Film, bool>> filter = null,
            Func<IQueryable<Film>, IOrderedQueryable<Film>> orderBy = null);

        Task<IEnumerable<Film>> GetNewsAsync();
        Task<IEnumerable<Film>> FindByTagAsync(int id);
        Task<IEnumerable<Film>> FindByTagsAsync(List<int> tags, int? filmId);
        Task<IEnumerable<Film>> FindBySubjectAsync(int subject, int? filmId);
        Task<IEnumerable<Film>> FindByTagsAndSubjectAsync(List<int> tags, string subject, int? filmId);
        Task<IEnumerable<Film>> FindWithOneTagAsync(int tagId);
        Task<IEnumerable<Film>> FindByUploaderId(string uploaderId);

        Task<IEnumerable<Film>> SearchAsync(string statement);
    }
}