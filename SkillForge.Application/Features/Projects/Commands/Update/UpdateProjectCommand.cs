using MediatR;
using SkillForge.Application.Common.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Commands.Update
{
    public class UpdateProjectCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? RepositoryUrl { get; set; }
    }
}
