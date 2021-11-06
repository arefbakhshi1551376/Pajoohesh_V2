using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Model.Models.Main.Subject;

namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    public class SubjectRepository : ISubjectRepository
    {
        private ApplicationDbContext _context;

        public SubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subject> FindByTitleAsync(string title)
        {
            IQueryable<Subject> query = _context.Subjects
                .Where(s => s.Title == title)
                .Include(s => s.Creator)
                .Include(s => s.LastModifier);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Subject>> SearchAsync(string statement)
        {
            List<Subject> finalResult = new List<Subject>();
            var subjects = await GetAllAsync();
            foreach (var subject in subjects)
            {
                if (subject.Title.ToLower().Contains(statement))
                {
                    finalResult.Add(subject);
                }
            }
            return finalResult;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync(Expression<Func<Subject, bool>> filter = null,
            Func<IQueryable<Subject>, IOrderedQueryable<Subject>> orderBy = null)
        {
            IQueryable<Subject> query = _context.Subjects
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

        public async Task<Subject> FindAsync(Expression<Func<Subject, bool>> filter = null)
        {
            IQueryable<Subject> query = _context.Subjects
                .Include(s => s.Creator)
                .Include(s => s.LastModifier);
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<Subject, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(Subject entity)
        {
            await _context.Subjects.AddAsync(entity);
        }

        public async Task Update(Subject entity)
        {
            var currentSubject = await FindAsync(e => e.Id == entity.Id);
            currentSubject.Title = entity.Title;
            currentSubject.ImageUrl = entity.ImageUrl;
            currentSubject.LastModifier = entity.LastModifier;
            currentSubject.LastModifyDate = entity.LastModifyDate;
            currentSubject.State = entity.State;
        }

        public async Task DeleteAsync(Subject entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var subject = await FindAsync(e => e.Id == id);
            _context.Subjects.Remove(subject);
        }
    }
}