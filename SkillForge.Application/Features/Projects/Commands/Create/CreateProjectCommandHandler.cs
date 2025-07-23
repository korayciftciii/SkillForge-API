using MediatR;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Commands.Create
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
    {
        private readonly AppDbContext _context;

        public CreateProjectCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = new Project
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                RepositoryUrl = request.RepositoryUrl
            };

            await _context.Projects.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new ProjectDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                RepositoryUrl = entity.RepositoryUrl
            };

            return Result<ProjectDto>.Ok(dto, "Project created successfully.");
        }
    }
}
