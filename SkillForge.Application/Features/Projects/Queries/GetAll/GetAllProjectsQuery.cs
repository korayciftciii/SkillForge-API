using MediatR;
using SkillForge.Application.Common.Models.Pagination;
using SkillForge.Application.Common.Models.Results;
using SkillForge.Application.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Queries.GetAll
{
    public class GetAllProjectsQuery : IRequest<PaginatedResult<ProjectDto>>
    {
        public PaginationFilter Filter { get; set; }

        public GetAllProjectsQuery(PaginationFilter filter)
        {
            Filter = filter;
        }
    }
}
