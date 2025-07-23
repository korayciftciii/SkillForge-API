using MediatR;
using SkillForge.Application.Common.Interfaces;
using SkillForge.Application.DataTransferObjects;
using SkillForge.Domain.Entities;
using SkillForge.Shared.Results;
using System;
using System.Collections.Generic;

namespace SkillForge.Application.Features.ProjectMembers.Commands.Create
{
    public class CreateProjectMemberCommand : IRequest<Result<ProjectMemberDto>>, ICacheInvalidator
    {
        public string UserId { get; set; } = string.Empty;
        public ProjectMemberRole Role { get; set; } = ProjectMemberRole.Member;
        public Guid ProjectId { get; set; }
        public string? InvitedBy { get; set; }
        
        // Cache invalidation
        public IEnumerable<string> CacheKeysToInvalidate => new[] { "project-members-all", $"project-{ProjectId}-members" };
    }
} 