using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.ProjectTags.Queries.GetById
{
    public class GetProjectTagByIdQueryHandler : IRequestHandler<GetProjectTagByIdQuery, Result<ProjectTagDto>>
    {
        private readonly AppDbContext _context;

        public GetProjectTagByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProjectTagDto>> Handle(GetProjectTagByIdQuery request, CancellationToken cancellationToken)
        {
            var projectTag = await _context.ProjectTags
                .Include(pt => pt.Project)
                .AsNoTracking()
                .Where(pt => pt.Id == request.Id)
                .Select(pt => new ProjectTagDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    Description = pt.Description,
                    ProjectId = pt.ProjectId,
                    ProjectTitle = pt.Project.Title
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (projectTag is null)
                return Result<ProjectTagDto>.Fail("Project tag not found.");

            return Result<ProjectTagDto>.Ok(projectTag);
        }
    }
} 