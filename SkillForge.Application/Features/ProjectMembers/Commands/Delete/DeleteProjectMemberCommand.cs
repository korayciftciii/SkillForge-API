using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Delete
{
    public class DeleteProjectMemberCommand : IRequest<Result>, ICacheInvalidator
    {
        public int Id { get; set; }
        public Guid ProjectId { get; set; }

        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "project-members-all", $"project-{ProjectId}-members", $"project-member-{Id}" };

        public DeleteProjectMemberCommand(int id, Guid projectId)
        {
            Id = id;
            ProjectId = projectId;
        }
    }
} 