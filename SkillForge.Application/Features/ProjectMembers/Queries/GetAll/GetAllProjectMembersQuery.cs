using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.Common.Models.Pagination;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using System;

namespace SkillForge.Application.Features.ProjectMembers.Queries.GetAll
{
    public class GetAllProjectMembersQuery : IRequest<PaginatedResult<ProjectMemberDto>>, ICacheableQuery<PaginatedResult<ProjectMemberDto>>
    {
        public PaginationFilter Filter { get; set; }
        public Guid? ProjectId { get; set; }
        public bool BypassCache { get; private set; }
        public string CacheKey => $"project-members-page-{Filter.Page}-size-{Filter.PageSize}-project-{ProjectId ?? Guid.Empty}-search-{Filter.Search ?? "none"}";
        public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(5);

        public GetAllProjectMembersQuery(PaginationFilter filter, Guid? projectId = null, bool bypassCache = false)
        {
            Filter = filter;
            ProjectId = projectId;
            BypassCache = bypassCache;
        }
    }
} 