using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Commands.Delete
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Result>
    {
        private readonly AppDbContext _context;

        public DeleteProjectCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(new object[] { request.Id }, cancellationToken);

            if (project == null)
                return Result.Fail("Project not found.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok("Project deleted successfully.");
        }
    }
}
