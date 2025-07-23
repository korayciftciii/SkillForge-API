using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SkillForge.Shared.Utilities;

namespace SkillForge.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => GetClaimValue(ClaimTypes.NameIdentifier);
        
        public string? UserName => GetClaimValue(ClaimTypes.Name);
        
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        private string? GetClaimValue(string claimType)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
} 