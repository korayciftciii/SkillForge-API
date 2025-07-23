using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;

namespace SkillForge.Application.Features.Users.Queries.GetById
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>, ICacheableQuery<Result<UserDto>>
    {
        public string Id { get; set; } = string.Empty;
        public bool BypassCache { get; private set; }
        public string CacheKey => $"user-{Id}";
        public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(10);

        public GetUserByIdQuery(string id, bool bypassCache = false)
        {
            Id = id;
            BypassCache = bypassCache;
        }
    }
} 