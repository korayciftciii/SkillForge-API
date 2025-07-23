using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;

namespace SkillForge.Application.Features.ProjectMembers.Queries.GetById
{
    public class GetProjectMemberByIdQuery : IRequest<Result<ProjectMemberDto>>, ICacheableQuery<Result<ProjectMemberDto>>
    {
        public int Id { get; set; }
        public bool BypassCache { get; private set; }
        public string CacheKey => $"project-member-{Id}";
        public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(10);

        public GetProjectMemberByIdQuery(int id, bool bypassCache = false)
        {
            Id = id;
            BypassCache = bypassCache;
        }
    }
} 