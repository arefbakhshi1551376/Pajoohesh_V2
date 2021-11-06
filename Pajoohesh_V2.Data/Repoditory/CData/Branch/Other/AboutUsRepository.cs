using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Model.Models.Main.AboutUs;

namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    class AboutUsRepository : IAboutUsRepository
    {
        private ApplicationDbContext _context;

        public AboutUsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AboutUs>> GetAllAsync(Expression<Func<AboutUs, bool>> filter = null, Func<IQueryable<AboutUs>, IOrderedQueryable<AboutUs>> orderBy = null)
        {
            IQueryable<AboutUs> query = _context.AboutUs
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

        public async Task<AboutUs> FindAsync(Expression<Func<AboutUs, bool>> filter = null)
        {
            IQueryable<AboutUs> query = _context.AboutUs
                .Include(s => s.Creator)
                .Include(s => s.LastModifier);
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<AboutUs, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(AboutUs entity)
        {
            await _context.AboutUs.AddAsync(entity);
        }

        public async Task Update(AboutUs entity)
        {
            var currentAboutUs = await FindAsync(au => au.Id == entity.Id);
            currentAboutUs.Title = entity.Title;
            currentAboutUs.Text = entity.Text;
            currentAboutUs.LastModifier = entity.LastModifier;
            currentAboutUs.LastModifyDate = entity.LastModifyDate;
        }

        public async Task DeleteAsync(AboutUs entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var currentAboutUs = await FindAsync(au => au.Id == id);
            _context.AboutUs.Remove(currentAboutUs);
        }
    }
}
