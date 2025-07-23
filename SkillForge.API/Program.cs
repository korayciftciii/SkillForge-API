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
using FluentValidation;
using SkillForge.API.Middlewares;
using Microsoft.OpenApi.Models;
using SkillForge.API.Extensions;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

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
                Window = TimeSpan.FromSeconds(30), // 30 saniyede bir sýfýrlanýr
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// JWT Settings configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Identity servisleri
builder.Services.AddIdentityInfrastructure(builder.Configuration);

// AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth();


builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    db.Database.Migrate();

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

app.Run();