using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectTags.Commands.Create
{
    public class CreateProjectTagCommandHandler : IRequestHandler<CreateProjectTagCommand, Result<ProjectTagDto>>
    {
        private readonly AppDbContext _context;

        public CreateProjectTagCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProjectTagDto>> Handle(CreateProjectTagCommand request, CancellationToken cancellationToken)
        {
            // Check if project exists
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (!projectExists)
                return Result<ProjectTagDto>.Fail("Project not found.");

            // Check if tag with same name already exists for this project
            var existingTag = await _context.ProjectTags
                .FirstOrDefaultAsync(pt => pt.ProjectId == request.ProjectId && pt.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (existingTag != null)
                return Result<ProjectTagDto>.Fail("A tag with this name already exists for this project.");

            var entity = new ProjectTag
            {
                Name = request.Name,
                Description = request.Description,
                ProjectId = request.ProjectId
            };

            await _context.ProjectTags.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Get project title for DTO
            var project = await _context.Projects
                .Where(p => p.Id == request.ProjectId)
                .Select(p => p.Title)
                .FirstOrDefaultAsync(cancellationToken);

            var dto = new ProjectTagDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ProjectId = entity.ProjectId,
                ProjectTitle = project
            };

            return Result<ProjectTagDto>.Ok(dto, "Project tag created successfully.");
        }
    }
} 