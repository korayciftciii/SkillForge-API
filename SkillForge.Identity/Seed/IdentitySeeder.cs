using Microsoft.AspNetCore.Identity;
using SkillForge.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Identity.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            var adminEmail = "korayciftciii60@gmail.com";
            var adminPassword = "#Rtz45Sky"; // geçici şifre, sonra environment’a alınabilir

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin",
                    Surname="system admin",
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
