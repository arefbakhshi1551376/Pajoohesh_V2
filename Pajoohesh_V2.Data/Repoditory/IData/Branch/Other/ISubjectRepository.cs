using System.Collections.Generic;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Main;
using Pajoohesh_V2.Model.Models.Main.Subject;

namespace Pajoohesh_V2.Data.Repoditory.IData.Branch.Other
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject> FindByTitleAsync(string title);
        Task<IEnumerable<Subject>> SearchAsync(string statement);
    }
}