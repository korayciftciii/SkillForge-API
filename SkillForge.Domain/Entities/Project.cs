using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillForge.Domain.Common;

namespace SkillForge.Domain.Entities
{
    public class Project : AuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? RepositoryUrl { get; set; }
        
        // Owner information
        public string OwnerId { get; set; } = string.Empty;
        
        // Project status
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
        
        // Project category
        public string? Category { get; set; }
        
        // Project tags
        public List<ProjectTag> Tags { get; set; } = new();
        
        // Team members
        public List<ProjectMember> TeamMembers { get; set; } = new();

        // Is the project visible to public
        public bool IsPublic { get; set; } = true;
    }
    
    public enum ProjectStatus
    {
        Planning = 0,
        InProgress = 1,
        Completed = 2,
        OnHold = 3,
        Canceled = 4
    }
}
