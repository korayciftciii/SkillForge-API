using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Queries.GetAll
{

    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PaginatedResult<ProjectDto>>
    {
        private readonly AppDbContext _context;

        public GetAllProjectsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Filter.Search))
            {
                query = query.Where(p =>
                    p.Title.ToLower().Contains(request.Filter.Search.ToLower()) ||
                    p.Description.ToLower().Contains(request.Filter.Search.ToLower()));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            bool desc = request.Filter.SortDirection.ToLower() == "desc";
            query = desc
                ? query.OrderByDescending(p => EF.Property<object>(p, request.Filter.SortBy))
                : query.OrderBy(p => EF.Property<object>(p, request.Filter.SortBy));

            var data = await query
                .Skip((request.Filter.Page - 1) * request.Filter.PageSize)
                .Take(request.Filter.PageSize)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    RepositoryUrl = p.RepositoryUrl
                })
                .ToListAsync(cancellationToken);

            return new PaginatedResult<ProjectDto>(data, totalCount, request.Filter.Page, request.Filter.PageSize);
        }
    }

}
