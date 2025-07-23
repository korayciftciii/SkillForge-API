using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Results;
using System.Collections.Generic;

namespace SkillForge.Application.Features.Users.Commands.Update
{
    public class UpdateUserCommand : IRequest<Result>, ICacheInvalidator
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public List<string> Roles { get; set; } = new();
        
        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "users-all", $"user-{Id}" };
    }
} 