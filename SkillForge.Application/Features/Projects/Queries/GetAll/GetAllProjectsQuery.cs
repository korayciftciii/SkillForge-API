using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.Common.Models.Pagination;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Queries.GetAll
{
    public class GetAllProjectsQuery : IRequest<PaginatedResult<ProjectDto>>, ICacheableQuery<PaginatedResult<ProjectDto>>
    {
        public PaginationFilter Filter { get; set; }
        public bool BypassCache { get; private set; }
        public string CacheKey => $"projects-page-{Filter.Page}-size-{Filter.PageSize}-search-{Filter.Search ?? "none"}-sort-{Filter.SortBy ?? "default"}-{Filter.SortDirection ?? "desc"}";
        public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(5);

        public GetAllProjectsQuery(PaginationFilter filter, bool bypassCache = false)
        {
            Filter = filter;
            BypassCache = bypassCache;
        }
    }
}
