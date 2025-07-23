using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Queries.GetById
{
    public class GetProjectByIdQuery : IRequest<Result<ProjectDto>>, ICacheableQuery<Result<ProjectDto>>
    {
        public Guid Id { get; set; }
        public bool BypassCache { get; private set; }
        public string CacheKey => $"project-{Id}";
        public TimeSpan? CacheExpiration => TimeSpan.FromMinutes(10);

        public GetProjectByIdQuery(Guid id, bool bypassCache = false)
        {
            Id = id;
            BypassCache = bypassCache;
        }
    }
}
