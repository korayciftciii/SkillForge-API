using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Features.Projects.Commands.Create
{
    public class CreateProjectCommand : IRequest<Result<ProjectDto>>, ICacheInvalidator
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? RepositoryUrl { get; set; }
        
        // Owner information
        public string? OwnerId { get; set; }
        
        // Category information
        public string? Category { get; set; }
        
        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "projects-all" };
    }
}
