using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;

namespace SkillForge.Application.Features.ProjectTags.Queries.GetById
{
    public class GetProjectTagByIdQuery : IRequest<Result<ProjectTagDto>>, ICacheableQuery<Result<ProjectTagDto>>
    {
        public int Id { get; set; }
        public bool BypassCache { get; private set; }
        public string CacheKey => $"project-tag-{Id}";
        public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(10);

        public GetProjectTagByIdQuery(int id, bool bypassCache = false)
        {
            Id = id;
            BypassCache = bypassCache;
        }
    }
} 