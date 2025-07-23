using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Update
{
    public class UpdateProjectMemberCommandHandler : IRequestHandler<UpdateProjectMemberCommand, Result>
    {
        private readonly AppDbContext _context;

        public UpdateProjectMemberCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateProjectMemberCommand request, CancellationToken cancellationToken)
        {
            var projectMember = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.Id == request.Id, cancellationToken);

            if (projectMember == null)
                return Result.Fail("Project member not found.");

            // Verify the project ID matches
            if (projectMember.ProjectId != request.ProjectId)
                return Result.Fail("Project ID mismatch.");

            projectMember.Role = request.Role;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Ok("Project member updated successfully.");
        }
    }
} 