using Microsoft.AspNetCore.Identity;
using Model.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class DatingSiteContextInitializer
    {
        public static void InitializeRoles(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                roleManager.CreateAsync(new IdentityRole("ADMIN")).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole("USER")).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole("CAMGIRL")).GetAwaiter().GetResult();
            }
        }

        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            InitializeRoles(userManager, roleManager);
            // Add any additional seed data logic here
        }
    }
}
