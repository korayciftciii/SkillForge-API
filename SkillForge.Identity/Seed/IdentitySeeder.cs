using Microsoft.AspNetCore.Identity;
using SkillForge.Identity.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Identity.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminUserAsync(
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager,
            ILogger? logger = null)
        {
            try
            {
                var adminEmail = "admin@gmail.com";
                var adminPassword = "Admin12345."; // geçici şifre, sonra environment'a alınabilir (10+ characters)

                logger?.LogInformation("Starting admin user seeding for email: {Email}", adminEmail);

                // Create all predefined roles
                await CreateRolesAsync(roleManager, logger);

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    logger?.LogInformation("Admin user does not exist. Creating...");
                    
                    var user = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        Name = "Admin",
                        Surname = "System Admin",
                        EmailConfirmed = true // Add this to ensure email is confirmed
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);

                    if (result.Succeeded)
                    {
                        logger?.LogInformation("Admin user created successfully. Adding to Admin role...");
                        
                        var addToRoleResult = await userManager.AddToRoleAsync(user, ApplicationRole.Admin);
                        if (addToRoleResult.Succeeded)
                        {
                            logger?.LogInformation("Admin user successfully added to Admin role.");
                        }
                        else
                        {
                            logger?.LogError("Failed to add admin user to Admin role. Errors: {Errors}", 
                                string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger?.LogError("Failed to create admin user. Errors: {Errors}", 
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger?.LogInformation("Admin user already exists with email: {Email}", adminEmail);
                    
                    // Check if user is in Admin role
                    var isInRole = await userManager.IsInRoleAsync(adminUser, ApplicationRole.Admin);
                    if (!isInRole)
                    {
                        logger?.LogInformation("Admin user exists but not in Admin role. Adding...");
                        var addToRoleResult = await userManager.AddToRoleAsync(adminUser, ApplicationRole.Admin);
                        if (addToRoleResult.Succeeded)
                        {
                            logger?.LogInformation("Existing admin user successfully added to Admin role.");
                        }
                        else
                        {
                            logger?.LogError("Failed to add existing admin user to Admin role. Errors: {Errors}", 
                                string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger?.LogInformation("Admin user already exists and is in Admin role.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred during admin user seeding: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        private static async Task CreateRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger? logger = null)
        {
            var roles = ApplicationRole.GetAllRoles();

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    logger?.LogInformation("{Role} role does not exist. Creating...", roleName);
                    var roleResult = await roleManager.CreateAsync(new ApplicationRole(roleName));
                    if (roleResult.Succeeded)
                    {
                        logger?.LogInformation("{Role} role created successfully.", roleName);
                    }
                    else
                    {
                        logger?.LogError("Failed to create {Role} role. Errors: {Errors}", 
                            roleName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger?.LogInformation("{Role} role already exists.", roleName);
                }
            }
        }
    }
}
