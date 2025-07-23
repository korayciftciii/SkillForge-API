using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SkillForge.Shared.Configuration
{
    public static class ConfigurationExtensions
    {
        public static AppSecrets GetAppSecrets(this IConfiguration configuration, bool isProduction)
        {
            if (isProduction)
            {
                return new AppSecrets
                {
                    Database = DatabaseSecrets.FromEnvironment(),
                    Jwt = JwtSecrets.FromEnvironment()
                };
            }
            
            var secrets = new AppSecrets();
            
            // Load from configuration for development
            var dbSection = configuration.GetSection("ConnectionStrings");
            if (dbSection.Exists())
            {
                secrets.Database.DefaultConnection = dbSection["DefaultConnection"] ?? string.Empty;
                secrets.Database.ProjectDb = dbSection["ProjectDb"] ?? string.Empty;
            }
            
            var jwtSection = configuration.GetSection("Jwt");
            if (jwtSection.Exists())
            {
                secrets.Jwt.Key = jwtSection["Key"] ?? string.Empty;
                secrets.Jwt.Issuer = jwtSection["Issuer"] ?? string.Empty;
                secrets.Jwt.Audience = jwtSection["Audience"] ?? string.Empty;
                
                if (int.TryParse(jwtSection["ExpiresInMinutes"], out int minutes))
                {
                    secrets.Jwt.ExpiresInMinutes = minutes;
                }
            }
            
            return secrets;
        }

        public static IServiceCollection AddAppSecrets(this IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            var secrets = configuration.GetAppSecrets(isProduction);
            services.AddSingleton(secrets);
            return services;
        }
    }
} 