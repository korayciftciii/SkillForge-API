using System;
using System.Collections.Generic;

namespace SkillForge.Domain.Entities
{
    public class ProjectMember
    {
        public int Id { get; set; }
        
        // User ID from Identity
        public string UserId { get; set; } = string.Empty;
        
        // Member role in the project
        public ProjectMemberRole Role { get; set; } = ProjectMemberRole.Member;
        
        // Navigation properties
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        
        // Audit fields
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public string? InvitedBy { get; set; }
    }
    
    public enum ProjectMemberRole
    {
        Owner = 0,
        Manager = 1,
        Member = 2,
        Viewer = 3
    }
} 