using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using SkillForge.Infrastructure.Caching;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Infrastructure.Services;
using SkillForge.Shared.Caching;
using SkillForge.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            string connectionString,
            bool useRedisCache = false,
            string? redisConnectionString = null)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            // Register HttpContextAccessor manually
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            // Register CurrentUserService
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            
            // Register DateTimeProvider
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            
            // Configure caching
            if (useRedisCache && !string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = "SkillForge:";
                });
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ProjectDb") ?? 
                throw new InvalidOperationException("Connection string 'ProjectDb' not found.");
                
            var useRedisCache = configuration.GetValue<bool>("Cache:UseRedis");
            var redisConnectionString = configuration.GetValue<string>("Cache:RedisConnectionString");
            
            return AddInfrastructure(services, connectionString, useRedisCache, redisConnectionString);
        }
    }
}
