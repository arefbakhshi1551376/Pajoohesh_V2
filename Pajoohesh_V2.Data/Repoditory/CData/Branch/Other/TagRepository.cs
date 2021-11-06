using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Model.Models.Main.Tag;

namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    public class TagRepository : ITagRepository
    {
        private ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> FindByTitleAsync(string title)
        {
            IQueryable<Tag> query = _context.Tags
                .Where(s => s.Title == title)
                .Include(s => s.Creator)
                .Include(s => s.LastModifier);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Tag>> SearchAsync(string statement)
        {
            List<Tag> finalResult = new List<Tag>();
            var tags = await GetAllAsync();
            foreach (var tag in tags)
            {
                if (tag.Title.ToLower().Contains(statement))
                {
                    finalResult.Add(tag);
                }
            }
            return finalResult;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync(Expression<Func<Tag, bool>> filter = null, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy = null)
        {
            IQueryable<Tag> query = _context.Tags
                .Include(s => s.Creator)
                .Include(s => s.LastModifier);
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

        public async Task<Tag> FindAsync(Expression<Func<Tag, bool>> filter = null)
        {
            IQueryable<Tag> query = _context.Tags
                .Include(s => s.Creator)
                .Include(s => s.LastModifier);
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<Tag, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(Tag entity)
        {
            await _context.Tags.AddAsync(entity);
        }

        public async Task Update(Tag entity)
        {
            var currentTag = await FindAsync(e => e.Id == entity.Id);
            currentTag.Title = entity.Title;
            currentTag.LastModifier = entity.LastModifier;
            currentTag.LastModifyDate = entity.LastModifyDate;
            currentTag.State = entity.State;
        }

        public async Task DeleteAsync(Tag entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await FindAsync(e => e.Id == id);
            _context.Tags.Remove(tag);
        }
    }
}