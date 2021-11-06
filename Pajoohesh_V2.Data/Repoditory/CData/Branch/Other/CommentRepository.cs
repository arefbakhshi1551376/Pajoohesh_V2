using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Model.Models.Main.Comment;

namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    public class CommentRepository : ICommentRepository
    {
        private ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync(Expression<Func<Comment, bool>> filter = null, Func<IQueryable<Comment>, IOrderedQueryable<Comment>> orderBy = null)
        {
            IQueryable<Comment> query = _context
                .Comments
                .Include(c => c.Film)
                .Include(c => c.Creator);

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

        public async Task<Comment> FindAsync(Expression<Func<Comment, bool>> filter = null)
        {
            IQueryable<Comment> query = _context
                .Comments
                .Include(c => c.Film)
                .Include(c => c.Creator);
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<Comment, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(Comment entity)
        {
            await _context.Comments.AddAsync(entity);
        }

        public async Task Update(Comment entity)
        {
            var currentComment = await FindAsync(c => c.Id == entity.Id);
            currentComment.Text = entity.Text;
            currentComment.Film = entity.Film;
            currentComment.LaseModifyDate = entity.LaseModifyDate;
        }

        public async Task DeleteAsync(Comment entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await FindAsync(c => c.Id == id);
            _context.Comments.Remove(comment);
        }
    }
}