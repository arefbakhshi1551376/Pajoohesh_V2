using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Data.Repoditory.IData.Branch.Other;
using Pajoohesh_V2.Model.Models.Main.User;


namespace Pajoohesh_V2.Data.Repoditory.CData.Branch.Other
{
    public class UserRepository : IUserRepository
    {
        private UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<User> FindAsync(string id)
        {
            var user = await _userManager
                .Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<int> GetCount(IEnumerable<User> users = null)
        {
            int userCount;

            if (users==null)
            {
                userCount = (await GetAllAsync()).Count();
            }
            else
            {
                userCount = users.Count();
            }

            return userCount;
        }

        public async Task<bool> AddAsync(User user, string password)
        {
            bool finalResult = false;
            var identityResult = await _userManager.CreateAsync(user, password.Trim());
            if (identityResult.Succeeded)
            {
                var claimResult = await _userManager.AddClaimAsync(user, new Claim("UserType", "UsualUser"));
                finalResult = true;
            }

            return finalResult;
        }

        public async Task UpdateAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> DeleteAsync(User user)
        {
            bool finalResult = false;
            try
            {
                await DeleteAsync(user.Id);
                finalResult = true;
            }
            catch (Exception e)
            {
                finalResult = false;
            }

            return finalResult;
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
        }

        public async Task DisconnectAccess(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.IsAccessValid = false;
        }

        public async Task DisconnectAccess(User user)
        {
            await DisconnectAccess(user.Id);
        }

        public async Task ConnectAccess(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.IsAccessValid = true;
        }

        public async Task ConnectAccess(User user)
        {
            await ConnectAccess(user.Id);
        }
    }
}
