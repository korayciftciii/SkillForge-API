using MediatR;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Queries.GetById
{
    public class GetProjectByIdQuery : IRequest<Result<ProjectDto>>
    {
        public Guid Id { get; set; }

        public GetProjectByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
