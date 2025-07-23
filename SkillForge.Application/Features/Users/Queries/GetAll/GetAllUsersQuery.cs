using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.Common.Models.Pagination;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using System;

namespace SkillForge.Application.Features.Users.Queries.GetAll
{
    public class GetAllUsersQuery : IRequest<PaginatedResult<UserDto>>, ICacheableQuery<PaginatedResult<UserDto>>
    {
        public PaginationFilter Filter { get; set; }
        public bool BypassCache { get; private set; }
        public string CacheKey => $"users-page-{Filter.Page}-size-{Filter.PageSize}-search-{Filter.Search ?? "none"}-sort-{Filter.SortBy ?? "default"}-{Filter.SortDirection ?? "desc"}";
        public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(5);

        public GetAllUsersQuery(PaginationFilter filter, bool bypassCache = false)
        {
            Filter = filter;
            BypassCache = bypassCache;
        }
    }
} 