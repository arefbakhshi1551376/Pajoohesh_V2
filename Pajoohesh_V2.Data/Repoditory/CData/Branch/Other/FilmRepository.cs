using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Tag;

namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    public class FilmRepository : IFilmRepository
    {
        private ApplicationDbContext _context;

        public FilmRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Film>> GetAllAsync(int skipNumber, int takeNumber,
            Expression<Func<Film, bool>> filter = null, Func<IQueryable<Film>, IOrderedQueryable<Film>> orderBy = null)
        {
            IQueryable<Film> query = _context.Films
                .Include(f => f.Comments)
                .Include(f => f.Subject)
                .Include(f => f.FilmTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.Uploader)
                .Include(f => f.LastModifier)
                .Include(f => f.LikeUsers)
                .Skip(skipNumber)
                .Take(takeNumber);
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

        public async Task<IEnumerable<Film>> GetNewsAsync()
        {
            var films = await GetAllAsync(0, 20);
            return films;
        }

        public async Task<IEnumerable<Film>> FindByTagAsync(int id)
        {
            var currentTag = await _context.Tags.FindAsync(id);
            IQueryable<Film> query = _context.Films
                .Include(f => f.Comments)
                .Include(f => f.Subject)
                .Include(f => f.FilmTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.Uploader)
                .Include(f => f.LastModifier)
                .Include(f => f.LikeUsers)
                .Where(f => f.FilmTags.Select(t => t.Tag).Contains(currentTag));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Film>> FindByTagsAsync(List<int> tags, int? filmId)
        {
            var films = (await GetAllAsync()).ToList();

            var finalFilms = new List<Film>();
            foreach (var tag in tags)
            {
                var currentTag = await _context.Tags.FindAsync(tag);
                foreach (var film in films)
                {
                    var filmTags = film.FilmTags.Select(f => f.Tag).ToList();
                    if (filmTags.Contains(currentTag) && film.Id != filmId)
                    {
                        finalFilms.Add(film);
                    }
                }
            }

            return finalFilms.Distinct();
        }

        public async Task<IEnumerable<Film>> FindBySubjectAsync(int subject, int? filmId)
        {
            var films = (await GetAllAsync()).ToList();

            var finalFilms = new List<Film>();

            var currentSubject = await _context.Subjects.FindAsync(subject);
            if (filmId == null)
            {
                foreach (var film in films)
                {
                    if (film.Subject == currentSubject)
                    {
                        finalFilms.Add(film);
                    }
                }
            }
            else
            {
                foreach (var film in films)
                {
                    if (film.Subject == currentSubject && film.Id != filmId)
                    {
                        finalFilms.Add(film);
                    }
                }
            }

            return finalFilms.Distinct();
        }

        public async Task<IEnumerable<Film>> FindByTagsAndSubjectAsync(List<int> tags, string subject, int? filmId)
        {
            var films = (await GetAllAsync()).ToList();

            var finalFilms = new List<Film>();
            foreach (var tag in tags)
            {
                var currentTag = await _context.Tags.FindAsync(tag);
                foreach (var film in films)
                {
                    var filmTags = film.FilmTags.Select(f => f.Tag).ToList();
                    if (filmTags.Contains(currentTag) && film.Id != filmId)
                    {
                        finalFilms.Add(film);
                    }
                    else if (film.Subject.Title == subject && film.Id != filmId)
                    {
                        finalFilms.Add(film);
                    }
                }
            }

            return finalFilms.Distinct();
        }

        public async Task<IEnumerable<Film>> FindWithOneTagAsync(int tagId)
        {
            var films = await _context.Films
                .Include(f => f.Comments)
                .Include(f => f.Subject)
                .Include(f => f.FilmTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.Uploader)
                .Include(f => f.LastModifier)
                .Include(f => f.LikeUsers)
                .Where(f => f.FilmTags.Count == 1 && f.FilmTags.FirstOrDefault().TagId == tagId)
                .ToListAsync();
            return films;
        }

        public async Task<IEnumerable<Film>> FindByUploaderId(string uploaderId)
        {
            var films = await _context.Films
                .Include(f => f.Comments)
                .Include(f => f.Subject)
                .Include(f => f.FilmTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.Uploader)
                .Include(f => f.LastModifier)
                .Include(f => f.LikeUsers)
                .Where(f => f.Uploader.Id==uploaderId)
                .ToListAsync();
            return films;
        }

        public async Task<IEnumerable<Film>> SearchAsync(string statement)
        {
            List<Film> finalResult = new List<Film>();

            TagRepository tagRepository = new TagRepository(_context);
            var currentTags =await tagRepository.SearchAsync(statement);

            var films = await GetAllAsync();
            foreach (var film in films)
            {
                if (film.Name.ToLower().Contains(statement))
                {
                    finalResult.Add(film);
                    continue;
                }
                else if (film.Subject.Title.ToLower().Contains(statement))
                {
                    finalResult.Add(film);
                    continue;
                }
                else
                {
                    foreach (var tag in currentTags)   
                    {
                        if (film.FilmTags.Select(t=>t.TagId).Contains(tag.Id))
                        {
                            finalResult.Add(film);
                        }
                    }
                }
            }

            return finalResult;
        }

        public async Task<IEnumerable<Film>> GetAllAsync(Expression<Func<Film, bool>> filter = null,
            Func<IQueryable<Film>, IOrderedQueryable<Film>> orderBy = null)
        {
            IQueryable<Film> query = _context.Films
                .Include(f => f.Comments)
                .Include(f => f.Subject)
                .Include(f => f.FilmTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.Uploader)
                .Include(f => f.LastModifier)
                .Include(f => f.LikeUsers);
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

        public async Task<Film> FindAsync(Expression<Func<Film, bool>> filter = null)
        {
            IQueryable<Film> query = _context.Films
                .Include(f => f.Comments)
                .Include(f => f.Subject)
                .Include(f => f.FilmTags)
                .ThenInclude(f => f.Tag)
                .Include(f => f.Uploader)
                .Include(f => f.LastModifier)
                .Include(f => f.LikeUsers);
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCount(Expression<Func<Film, bool>> filter = null)
        {
            var count = (await GetAllAsync(filter)).Count();
            return count;
        }

        public async Task AddAsync(Film entity)
        {
            await _context.Films.AddAsync(entity);
        }

        public async Task Update(Film entity)
        {
            var currentFilm = await FindAsync(e => e.Id == entity.Id);
            currentFilm.Comments = entity.Comments;
            currentFilm.Description = entity.Description;
            currentFilm.Name = entity.Name;
            currentFilm.State = entity.State;
            currentFilm.Subject = entity.Subject;
            currentFilm.FilmTags = entity.FilmTags;
            currentFilm.FilmUrl = entity.FilmUrl;
            currentFilm.ImageUrl = entity.ImageUrl;
            currentFilm.LastModifier = entity.LastModifier;
            currentFilm.LikeUsers = entity.LikeUsers;
            currentFilm.LaseModifyDate = entity.LaseModifyDate;
            currentFilm.NumberOfViews = entity.NumberOfViews;
        }

        public async Task DeleteAsync(Film entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var film = await FindAsync(e => e.Id == id);
            _context.Films.Remove(film);
        }
    }
}