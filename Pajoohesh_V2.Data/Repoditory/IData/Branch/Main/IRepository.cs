using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pajoohesh_V2.Model.Models.Main.Comment;

namespace Pajoohesh_V2.Data.Repoditory.IData.Branch.Main
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<T> FindAsync(Expression<Func<T, bool>> filter = null);
        Task<int> GetCount(Expression<Func<T, bool>> filter = null);
        Task AddAsync(T entity);
        Task Update(T entity);
        Task DeleteAsync(T entity);
        Task DeleteAsync(int id);
    }
}
