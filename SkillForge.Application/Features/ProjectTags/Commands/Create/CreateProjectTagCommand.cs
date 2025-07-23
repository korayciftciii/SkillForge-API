using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;

namespace SkillForge.Application.Features.ProjectTags.Commands.Create
{
    public class CreateProjectTagCommand : IRequest<Result<ProjectTagDto>>, ICacheInvalidator
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        
        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "project-tags-all", $"project-{ProjectId}-tags" };
    }
} 