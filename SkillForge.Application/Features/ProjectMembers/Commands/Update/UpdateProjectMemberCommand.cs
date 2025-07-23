using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Domain.Entities;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Update
{
    public class UpdateProjectMemberCommand : IRequest<Result>, ICacheInvalidator
    {
        public int Id { get; set; }
        public ProjectMemberRole Role { get; set; }
        public Guid ProjectId { get; set; }
        
        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "project-members-all", $"project-{ProjectId}-members", $"project-member-{Id}" };
    }
} 