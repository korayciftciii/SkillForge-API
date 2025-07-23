using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Infrastructure.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectTags.Queries.GetAll
{
    public class GetAllProjectTagsQueryHandler : IRequestHandler<GetAllProjectTagsQuery, PaginatedResult<ProjectTagDto>>
    {
        private readonly AppDbContext _context;

        public GetAllProjectTagsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<ProjectTagDto>> Handle(GetAllProjectTagsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ProjectTags
                .Include(pt => pt.Project)
                .AsQueryable();

            // Filter by project if specified
            if (request.ProjectId.HasValue)
            {
                query = query.Where(pt => pt.ProjectId == request.ProjectId.Value);
            }

            // Search functionality
            if (!string.IsNullOrWhiteSpace(request.Filter.Search))
            {
                query = query.Where(pt =>
                    pt.Name.ToLower().Contains(request.Filter.Search.ToLower()) ||
                    (pt.Description != null && pt.Description.ToLower().Contains(request.Filter.Search.ToLower())) ||
                    pt.Project.Title.ToLower().Contains(request.Filter.Search.ToLower()));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            bool desc = request.Filter.SortDirection?.ToLower() == "desc";
            query = request.Filter.SortBy?.ToLower() switch
            {
                "name" => desc ? query.OrderByDescending(pt => pt.Name) : query.OrderBy(pt => pt.Name),
                "description" => desc ? query.OrderByDescending(pt => pt.Description) : query.OrderBy(pt => pt.Description),
                "project" => desc ? query.OrderByDescending(pt => pt.Project.Title) : query.OrderBy(pt => pt.Project.Title),
                _ => desc ? query.OrderByDescending(pt => pt.Name) : query.OrderBy(pt => pt.Name)
            };

            var projectTags = await query
                .Skip((request.Filter.Page - 1) * request.Filter.PageSize)
                .Take(request.Filter.PageSize)
                .Select(pt => new ProjectTagDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    Description = pt.Description,
                    ProjectId = pt.ProjectId,
                    ProjectTitle = pt.Project.Title
                })
                .ToListAsync(cancellationToken);

            return new PaginatedResult<ProjectTagDto>(projectTags, totalCount, request.Filter.Page, request.Filter.PageSize);
        }
    }
} 