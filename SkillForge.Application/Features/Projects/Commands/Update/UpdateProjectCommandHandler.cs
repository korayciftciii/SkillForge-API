using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Commands.Update
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result>
    {
        private readonly AppDbContext _context;

        public UpdateProjectCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(request.Id);
            if (project == null)
                return Result.Fail("Project not found");

            project.Title = request.Title;
            project.Description = request.Description;
            project.RepositoryUrl = request.RepositoryUrl;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Ok("Project updated successfully");
        }
    }

}
