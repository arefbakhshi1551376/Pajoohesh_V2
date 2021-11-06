using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Model.Models.Main.ContactUs;

namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    public class ContactUsRepository : IContactUsRepository
    {
        private ApplicationDbContext _context;

        public ContactUsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactUs>> GetAllAsync(Expression<Func<ContactUs, bool>> filter = null, Func<IQueryable<ContactUs>, IOrderedQueryable<ContactUs>> orderBy = null)
        {
            IQueryable<ContactUs> query = _context.ContactUss
                .Include(s => s.Replier);
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

        public async Task<ContactUs> FindAsync(Expression<Func<ContactUs, bool>> filter = null)
        {
            IQueryable<ContactUs> query = _context.ContactUss
                .Include(s => s.Replier);
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<ContactUs, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(ContactUs entity)
        {
            await _context.ContactUss.AddAsync(entity);
            //await _context.SaveChangesAsync();
        }

        public async Task Update(ContactUs entity)
        {
            var currentContactUs = await FindAsync(e => e.Id == entity.Id);
            currentContactUs.IsReplied = entity.IsReplied;
            currentContactUs.Replier = entity.Replier;
            currentContactUs.ReplyDate = entity.ReplyDate;
            currentContactUs.ReplySubject = entity.ReplySubject;
            currentContactUs.ReplyText = entity.ReplyText;
        }

        public async Task DeleteAsync(ContactUs entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var currentContactUs = await FindAsync(e => e.Id == id);
            _context.ContactUss.Remove(currentContactUs);
        }

        public async Task<IEnumerable<ContactUs>> GetAllAsync(int skipNumber, int takeNumber, Expression<Func<ContactUs, bool>> filter = null, Func<IQueryable<ContactUs>, IOrderedQueryable<ContactUs>> orderBy = null)
        {
            IQueryable<ContactUs> query = _context.ContactUss
                .Include(s => s.Replier)
                /*.Skip(skipNumber)
                .Take(takeNumber)*/;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            var finalResult= await query.ToListAsync();
            return finalResult;
        }
    }
}
