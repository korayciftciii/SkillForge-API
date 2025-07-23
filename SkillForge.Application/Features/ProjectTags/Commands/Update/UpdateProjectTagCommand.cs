using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;

namespace SkillForge.Application.Features.ProjectTags.Commands.Update
{
    public class UpdateProjectTagCommand : IRequest<Result>, ICacheInvalidator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        
        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "project-tags-all", $"project-{ProjectId}-tags", $"project-tag-{Id}" };
    }
} 