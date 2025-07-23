using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Infrastructure.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectMembers.Queries.GetAll
{
    public class GetAllProjectMembersQueryHandler : IRequestHandler<GetAllProjectMembersQuery, PaginatedResult<ProjectMemberDto>>
    {
        private readonly AppDbContext _context;

        public GetAllProjectMembersQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<ProjectMemberDto>> Handle(GetAllProjectMembersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ProjectMembers
                .Include(pm => pm.Project)
                .AsQueryable();

            // Filter by project if specified
            if (request.ProjectId.HasValue)
            {
                query = query.Where(pm => pm.ProjectId == request.ProjectId.Value);
            }

            // Search functionality
            if (!string.IsNullOrWhiteSpace(request.Filter.Search))
            {
                query = query.Where(pm =>
                    pm.UserId.Contains(request.Filter.Search) ||
                    pm.Project.Title.ToLower().Contains(request.Filter.Search.ToLower()));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            bool desc = request.Filter.SortDirection?.ToLower() == "desc";
            query = request.Filter.SortBy?.ToLower() switch
            {
                "role" => desc ? query.OrderByDescending(pm => pm.Role) : query.OrderBy(pm => pm.Role),
                "joinedat" => desc ? query.OrderByDescending(pm => pm.JoinedAt) : query.OrderBy(pm => pm.JoinedAt),
                "userid" => desc ? query.OrderByDescending(pm => pm.UserId) : query.OrderBy(pm => pm.UserId),
                _ => desc ? query.OrderByDescending(pm => pm.JoinedAt) : query.OrderBy(pm => pm.JoinedAt)
            };

            var projectMembers = await query
                .Skip((request.Filter.Page - 1) * request.Filter.PageSize)
                .Take(request.Filter.PageSize)
                .Select(pm => new ProjectMemberDto
                {
                    Id = pm.Id,
                    UserId = pm.UserId,
                    Role = pm.Role,
                    ProjectId = pm.ProjectId,
                    JoinedAt = pm.JoinedAt,
                    InvitedBy = pm.InvitedBy,
                    ProjectTitle = pm.Project.Title
                })
                .ToListAsync(cancellationToken);

            return new PaginatedResult<ProjectMemberDto>(projectMembers, totalCount, request.Filter.Page, request.Filter.PageSize);
        }
    }
} 