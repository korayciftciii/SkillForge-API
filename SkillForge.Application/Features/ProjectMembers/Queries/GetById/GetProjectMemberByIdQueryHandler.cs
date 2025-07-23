using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectMembers.Queries.GetById
{
    public class GetProjectMemberByIdQueryHandler : IRequestHandler<GetProjectMemberByIdQuery, Result<ProjectMemberDto>>
    {
        private readonly AppDbContext _context;

        public GetProjectMemberByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProjectMemberDto>> Handle(GetProjectMemberByIdQuery request, CancellationToken cancellationToken)
        {
            var projectMember = await _context.ProjectMembers
                .Include(pm => pm.Project)
                .AsNoTracking()
                .Where(pm => pm.Id == request.Id)
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
                .FirstOrDefaultAsync(cancellationToken);

            if (projectMember is null)
                return Result<ProjectMemberDto>.Fail("Project member not found.");

            return Result<ProjectMemberDto>.Ok(projectMember);
        }
    }
} 