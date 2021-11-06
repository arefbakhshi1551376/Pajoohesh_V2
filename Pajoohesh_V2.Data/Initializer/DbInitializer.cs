using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Model.Models.Main.User;

namespace Pajoohesh_V2.Data.Initializer
{

    public class DbInitializer : IDbInitializer
    {
        private ApplicationDbContext _db;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task Initialize()
        {
            try
            {
                var isMigrationsExist = (await _db.Database.GetPendingMigrationsAsync()).Any();
                if (isMigrationsExist)
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }

            if (await _roleManager.FindByNameAsync(Constants.AdminRole) == null && await _roleManager.FindByNameAsync(Constants.ProgrammerRole) == null && await _roleManager.FindByNameAsync(Constants.UserRole) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(Constants.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(Constants.ProgrammerRole));
                await _roleManager.CreateAsync(new IdentityRole(Constants.UserRole));
            }
            
            var newUser = new User()
            {
                Email = Constants.DefaultEmail,
                Family = Constants.DefaultFamily,
                Name = Constants.DefaultName,
                IsAccessValid = true,
                IsEmailVerified = true,
                RegistryDate = DateTime.UtcNow,
                UserName = Constants.DefaultUserName,
                PhoneNumber = Constants.DefaultMobilePhoneNumber,
                IsPhoneNumberVerified = true,
                ImageUrl = "eeeeeeeeeeeeeeee.jpg"
            };

            var result = await _userManager.CreateAsync(newUser, Constants.DefaultPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, Constants.AdminRole);
                await _userManager.AddClaimAsync(newUser, new Claim(Constants.UserType, Constants.AdminClaim));
            }

        }
    }
}

