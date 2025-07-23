using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Create
{
    public class CreateProjectMemberCommandHandler : IRequestHandler<CreateProjectMemberCommand, Result<ProjectMemberDto>>
    {
        private readonly AppDbContext _context;

        public CreateProjectMemberCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProjectMemberDto>> Handle(CreateProjectMemberCommand request, CancellationToken cancellationToken)
        {
            // Check if project exists
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (!projectExists)
                return Result<ProjectMemberDto>.Fail("Project not found.");

            // Check if user is already a member of this project
            var existingMember = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == request.ProjectId && pm.UserId == request.UserId, cancellationToken);

            if (existingMember != null)
                return Result<ProjectMemberDto>.Fail("User is already a member of this project.");

            var entity = new ProjectMember
            {
                UserId = request.UserId,
                Role = request.Role,
                ProjectId = request.ProjectId,
                InvitedBy = request.InvitedBy,
                JoinedAt = DateTime.UtcNow
            };

            await _context.ProjectMembers.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Get project title for DTO
            var project = await _context.Projects
                .Where(p => p.Id == request.ProjectId)
                .Select(p => p.Title)
                .FirstOrDefaultAsync(cancellationToken);

            var dto = new ProjectMemberDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Role = entity.Role,
                ProjectId = entity.ProjectId,
                JoinedAt = entity.JoinedAt,
                InvitedBy = entity.InvitedBy,
                ProjectTitle = project
            };

            return Result<ProjectMemberDto>.Ok(dto, "Project member added successfully.");
        }
    }
} 