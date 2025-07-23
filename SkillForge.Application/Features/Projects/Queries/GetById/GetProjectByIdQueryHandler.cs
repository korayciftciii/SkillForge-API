using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Queries.GetById
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
    {
        private readonly AppDbContext _context;

        public GetProjectByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    RepositoryUrl = p.RepositoryUrl
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (project is null)
                return Result<ProjectDto>.Fail("Project not found.");

            return Result<ProjectDto>.Ok(project);
        }
    }
}
