using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillForge.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Identity
{
  public  class IdentityContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }
    }
}
