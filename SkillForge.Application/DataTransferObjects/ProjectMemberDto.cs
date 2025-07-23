using SkillForge.Domain.Entities;
using System;

namespace SkillForge.Application.DataTransferObjects
{
    public class ProjectMemberDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ProjectMemberRole Role { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime JoinedAt { get; set; }
        public string? InvitedBy { get; set; }
        
        // Optional project information
        public string? ProjectTitle { get; set; }
    }
} 