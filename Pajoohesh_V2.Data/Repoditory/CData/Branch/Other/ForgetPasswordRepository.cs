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
    public class ForgetPasswordRepository:IForgetPasswordRepository
    {
        private ApplicationDbContext _context;

        public ForgetPasswordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ForgetPassword>> GetAllAsync(Expression<Func<ForgetPassword, bool>> filter = null, Func<IQueryable<ForgetPassword>, IOrderedQueryable<ForgetPassword>> orderBy = null)
        {
            IQueryable<ForgetPassword> query = _context.ForgetPasswords
                .Include(s => s.User);
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

        public async Task<ForgetPassword> FindAsync(Expression<Func<ForgetPassword, bool>> filter = null)
        {
            IQueryable<ForgetPassword> query = _context.ForgetPasswords
                .Include(s => s.User);
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<ForgetPassword, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(ForgetPassword entity)
        {
            await _context.ForgetPasswords.AddAsync(entity);
        }

        public async Task Update(ForgetPassword entity)
        {
            var currentForgetPassword = await FindAsync(e => e.Id == entity.Id);
            currentForgetPassword.User = entity.User;
            currentForgetPassword.Key = entity.Key;
            currentForgetPassword.IsChangePasswordFinished = entity.IsChangePasswordFinished;
        }

        public async Task DeleteAsync(ForgetPassword entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var currentForgetPassword = await FindAsync(e => e.Id == id);
            _context.ForgetPasswords.Remove(currentForgetPassword);
        }
    }
}
