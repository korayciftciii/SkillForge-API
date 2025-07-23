using SkillForge.Identity;
using SkillForge.Identity.Configuration;
using SkillForge.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SkillForge.Identity.Models;
using SkillForge.Identity.Seed;
using SkillForge.Application;
using SkillForge.Infrastructure;
using SkillForge.Infrastructure.Persistence;
using FluentValidation;
using SkillForge.API.Middlewares;
using Microsoft.OpenApi.Models;
using SkillForge.API.Extensions;
using System.Threading.RateLimiting;
using SkillForge.Shared.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);
var isProduction = builder.Environment.IsProduction();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (isProduction)
{
    builder.Logging.AddEventLog();
}

// Secure configuration setup
builder.Services.AddAppSecrets(builder.Configuration, isProduction);
var appSecrets = builder.Configuration.GetAppSecrets(isProduction);

//Cors Policy
var allowedOrigins = "_allowedOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins,
        policy =>
        {
            policy.WithOrigins("https://skillforge-ui.vercel.app") // frontend adresi buraya
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, // Max 10 istek
                Window = TimeSpan.FromSeconds(30), // 30 saniyede bir sıfırlanır
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// JWT Settings configuration - use the secure configuration
builder.Services.Configure<JwtSettings>(opts => 
{
    opts.Key = appSecrets.Jwt.Key;
    opts.Issuer = appSecrets.Jwt.Issuer;
    opts.Audience = appSecrets.Jwt.Audience;
    opts.ExpiresInMinutes = appSecrets.Jwt.ExpiresInMinutes;
});

// Identity services - pass the secure connection string
builder.Services.AddIdentityInfrastructure(appSecrets.Database.DefaultConnection);

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // Set to true for better security
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = appSecrets.Jwt.Issuer,
        ValidAudience = appSecrets.Jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSecrets.Jwt.Key)),
        ClockSkew = TimeSpan.Zero,
        // Add additional validation parameters for better security
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth();

// First register application layer
builder.Services.AddApplication();

// Then register infrastructure layer with dependencies
builder.Services.AddInfrastructure(
    appSecrets.Database.ProjectDb, 
    builder.Configuration.GetValue<bool>("Cache:UseRedis"),
    builder.Configuration.GetValue<string>("Cache:RedisConnectionString"));

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SkillForge.Infrastructure.Persistence.AppDbContext>("application-db")
    .AddDbContextCheck<SkillForge.Identity.IdentityContext>("identity-db")
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "service" });

// Add cache health check if using Redis
if (builder.Configuration.GetValue<bool>("Cache:UseRedis"))
{
    var redisConnectionString = builder.Configuration.GetValue<string>("Cache:RedisConnectionString");
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        builder.Services.AddHealthChecks()
            .AddRedis(redisConnectionString, name: "redis-cache", tags: new[] { "cache" });
    }
}

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    // Migrate Identity database
    var identityDb = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    identityDb.Database.Migrate();
    
    // Migrate Application database
    var appDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDb.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    await IdentitySeeder.SeedAdminUserAsync(userManager, roleManager);
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseCors(allowedOrigins);
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

// Health Check Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("service") || healthCheck.Tags.Contains("database"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();