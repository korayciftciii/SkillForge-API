using MediatR;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Commands.Create
{
    public class CreateProjectCommand : IRequest<Result<ProjectDto>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? RepositoryUrl { get; set; }
    }
}
