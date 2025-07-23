using MediatR;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Commands.Delete
{
    public class DeleteProjectCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteProjectCommand(Guid id)
        {
            Id = id;
        }
    }
}
