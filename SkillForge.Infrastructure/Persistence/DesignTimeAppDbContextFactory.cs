using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SkillForge.Shared.Utilities;

namespace SkillForge.Infrastructure.Persistence
{
    public class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SkillForge.API"))
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("ProjectDb"));

            // For design-time, provide a default DateTimeProvider and null for CurrentUserService
            var dateTimeProvider = new DateTimeProvider();

            return new AppDbContext(optionsBuilder.Options, dateTimeProvider, null);
        }
    }
}
