using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;

namespace SkillForge.Application.Features.ProjectTags.Commands.Delete
{
    public class DeleteProjectTagCommand : IRequest<Result>, ICacheInvalidator
    {
        public int Id { get; set; }
        public Guid ProjectId { get; set; }

        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "project-tags-all", $"project-{ProjectId}-tags", $"project-tag-{Id}" };

        public DeleteProjectTagCommand(int id, Guid projectId)
        {
            Id = id;
            ProjectId = projectId;
        }
    }
} 