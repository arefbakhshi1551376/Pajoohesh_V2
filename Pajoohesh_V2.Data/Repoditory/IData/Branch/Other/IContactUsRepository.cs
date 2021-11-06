using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Main;
using Pajoohesh_V2.Model.Models.Main.ContactUs;
using Pajoohesh_V2.Model.Models.Main.Film;

namespace Pajoohesh_V2.Data.Repoditory.IData.Branch.Other
{
    public interface IContactUsRepository:IRepository<ContactUs>
    {
        Task<IEnumerable<ContactUs>> GetAllAsync(int skipNumber,
            int takeNumber, Expression<Func<ContactUs, bool>> filter = null,
            Func<IQueryable<ContactUs>, IOrderedQueryable<ContactUs>> orderBy = null);
    }
}
