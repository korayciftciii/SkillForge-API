using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Delete
{
    public class DeleteProjectMemberCommandHandler : IRequestHandler<DeleteProjectMemberCommand, Result>
    {
        private readonly AppDbContext _context;

        public DeleteProjectMemberCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteProjectMemberCommand request, CancellationToken cancellationToken)
        {
            var projectMember = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.Id == request.Id && pm.ProjectId == request.ProjectId, cancellationToken);

            if (projectMember == null)
                return Result.Fail("Project member not found.");

            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok("Project member removed successfully.");
        }
    }
} 