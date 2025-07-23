using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Results;
using System.Collections.Generic;

namespace SkillForge.Application.Features.Users.Commands.Delete
{
    public class DeleteUserCommand : IRequest<Result>, ICacheInvalidator
    {
        public string Id { get; set; } = string.Empty;

        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "users-all", $"user-{Id}" };

        public DeleteUserCommand(string id)
        {
            Id = id;
        }
    }
} 