using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectTags.Commands.Update
{
    public class UpdateProjectTagCommandHandler : IRequestHandler<UpdateProjectTagCommand, Result>
    {
        private readonly AppDbContext _context;

        public UpdateProjectTagCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateProjectTagCommand request, CancellationToken cancellationToken)
        {
            var projectTag = await _context.ProjectTags
                .FirstOrDefaultAsync(pt => pt.Id == request.Id, cancellationToken);

            if (projectTag == null)
                return Result.Fail("Project tag not found.");

            // Verify the project ID matches
            if (projectTag.ProjectId != request.ProjectId)
                return Result.Fail("Project ID mismatch.");

            // Check if another tag with the same name exists for this project
            var existingTag = await _context.ProjectTags
                .FirstOrDefaultAsync(pt => pt.ProjectId == request.ProjectId && 
                                          pt.Name.ToLower() == request.Name.ToLower() && 
                                          pt.Id != request.Id, cancellationToken);

            if (existingTag != null)
                return Result.Fail("A tag with this name already exists for this project.");

            projectTag.Name = request.Name;
            projectTag.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Ok("Project tag updated successfully.");
        }
    }
} 