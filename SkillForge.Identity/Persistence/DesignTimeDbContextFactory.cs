using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.IO;


namespace SkillForge.Identity.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
     .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SkillForge.API"))
     .AddJsonFile("appsettings.Development.json", optional: false)
     .Build();
            var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new IdentityContext(optionsBuilder.Options);
        }
    }
}
