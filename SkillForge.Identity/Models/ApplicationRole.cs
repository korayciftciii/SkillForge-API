using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Identity.Models
{
    public class ApplicationRole : IdentityRole
    {
        // Predefined role names
        public const string Admin = "Admin";
        public const string User = "User";

        // Constructor for role creation
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }

        // Get all available roles
        public static List<string> GetAllRoles()
        {
            return new List<string> { Admin, User };
        }
    }
}
