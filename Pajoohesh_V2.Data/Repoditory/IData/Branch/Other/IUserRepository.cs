using System.Collections.Generic;
using System.Threading.Tasks;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Main;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Data.Repoditory.IData.Branch.Other
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        //string id, string name = null, string lastName = null, string email = null, string role = null, bool? isAccessValid = null
        Task<User> FindAsync(string id);
        Task<int> GetCount(IEnumerable<User> users=null);

        //Insert
        Task<bool> AddAsync(User user, string password);

        //Update
        Task UpdateAsync(User user);

        //Delete
        Task<bool> DeleteAsync(User user);
        Task DeleteAsync(string id);

        //Disconnect access
        Task DisconnectAccess(string id);
        Task DisconnectAccess(User user);

        //Connect access
        Task ConnectAccess(string id);
        Task ConnectAccess(User user);
    }
}
