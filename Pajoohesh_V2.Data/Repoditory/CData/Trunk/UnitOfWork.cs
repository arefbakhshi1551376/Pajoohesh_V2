using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pajoohesh_V2.Data.Repoditory.CData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Data.Repoditory.IData.Trunk;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Data.Repoditory.CData.Trunk
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            CommentRepository = new CommentRepository(_context);
            FilmRepository = new FilmRepository(_context);
            SubjectRepository = new SubjectRepository(_context);
            TagRepository = new TagRepository(_context);
            ContactUsRepository = new ContactUsRepository(_context);
            AboutUsRepository = new AboutUsRepository(_context);
            ForgetPasswordRepository = new ForgetPasswordRepository(_context);
            NewsLetterRepository = new NewsLetterRepository(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public ICommentRepository CommentRepository { get; }
        public IFilmRepository FilmRepository { get; }
        public ISubjectRepository SubjectRepository { get; }
        public ITagRepository TagRepository { get; }
        public IContactUsRepository ContactUsRepository { get; }
        public IAboutUsRepository AboutUsRepository { get; }
        public IForgetPasswordRepository ForgetPasswordRepository { get; }
        public INewsLetterRepository NewsLetterRepository { get; }
    }
}
