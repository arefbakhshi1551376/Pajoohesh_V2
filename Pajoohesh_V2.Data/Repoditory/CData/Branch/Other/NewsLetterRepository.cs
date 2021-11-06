using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    public class NewsLetterRepository:INewsLetterRepository
    {
        private ApplicationDbContext _context;

        public NewsLetterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsLetter>> GetAllAsync(Expression<Func<NewsLetter, bool>> filter = null, Func<IQueryable<NewsLetter>, IOrderedQueryable<NewsLetter>> orderBy = null)
        {
            IQueryable<NewsLetter> query = _context.NewsLetters;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            return await query.ToListAsync();
        }

        public async Task<NewsLetter> FindAsync(Expression<Func<NewsLetter, bool>> filter = null)
        {
            IQueryable<NewsLetter> query = _context.NewsLetters;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<NewsLetter, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(NewsLetter entity)
        {
            await _context.NewsLetters.AddAsync(entity);
        }

        public async Task Update(NewsLetter entity)
        {
            var currentNewsLetter = await FindAsync(e => e.Id == entity.Id);
            currentNewsLetter.IsEmailVerified = entity.IsEmailVerified;
        }

        public async Task DeleteAsync(NewsLetter entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var currentForgetPassword = await FindAsync(e => e.Id == id);
            _context.NewsLetters.Remove(currentForgetPassword);
        }
    }
}
