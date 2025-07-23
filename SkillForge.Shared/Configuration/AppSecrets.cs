using System;

namespace SkillForge.Shared.Configuration
{
    public class AppSecrets
    {
        public DatabaseSecrets Database { get; set; } = new DatabaseSecrets();
        public JwtSecrets Jwt { get; set; } = new JwtSecrets();
    }

    public class DatabaseSecrets
    {
        public string DefaultConnection { get; set; } = string.Empty;
        public string ProjectDb { get; set; } = string.Empty;

        public static DatabaseSecrets FromEnvironment()
        {
            return new DatabaseSecrets
            {
                DefaultConnection = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? string.Empty,
                ProjectDb = Environment.GetEnvironmentVariable("PROJECT_DB_CONNECTION_STRING") ?? string.Empty
            };
        }
    }

    public class JwtSecrets
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; } = 60;

        public static JwtSecrets FromEnvironment()
        {
            return new JwtSecrets
            {
                Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? string.Empty,
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? string.Empty,
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? string.Empty,
                ExpiresInMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES"), out int minutes) ? minutes : 60
            };
        }
    }
} 