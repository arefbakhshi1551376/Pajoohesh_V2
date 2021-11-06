using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;

namespace Pajoohesh_V2.Data.Repoditory.IData.Trunk
{
    public interface IUnitOfWork : IDisposable
    {
        ICommentRepository CommentRepository { get; }
        IFilmRepository FilmRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        ITagRepository TagRepository { get; }
        IContactUsRepository ContactUsRepository { get; }
        IAboutUsRepository AboutUsRepository { get; }
        IForgetPasswordRepository ForgetPasswordRepository { get; }
        INewsLetterRepository NewsLetterRepository { get; }
        Task SaveAsync();
    }
}
