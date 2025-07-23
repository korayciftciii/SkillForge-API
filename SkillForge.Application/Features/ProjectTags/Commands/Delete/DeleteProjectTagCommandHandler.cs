using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectTags.Commands.Delete
{
    public class DeleteProjectTagCommandHandler : IRequestHandler<DeleteProjectTagCommand, Result>
    {
        private readonly AppDbContext _context;

        public DeleteProjectTagCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteProjectTagCommand request, CancellationToken cancellationToken)
        {
            var projectTag = await _context.ProjectTags
                .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.ProjectId == request.ProjectId, cancellationToken);

            if (projectTag == null)
                return Result.Fail("Project tag not found.");

            _context.ProjectTags.Remove(projectTag);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok("Project tag deleted successfully.");
        }
    }
} 