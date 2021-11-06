using System.Collections.Generic;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Main;
using Pajoohesh_V2.Model.Models.Main.Tag;

namespace Pajoohesh_V2.Data.Repoditory.IData.Branch.Other
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag> FindByTitleAsync(string title);
        Task<IEnumerable<Tag>> SearchAsync(string statement);
    }
}