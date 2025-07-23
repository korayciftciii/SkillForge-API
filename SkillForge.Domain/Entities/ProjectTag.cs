using System;
using System.Collections.Generic;

namespace SkillForge.Domain.Entities
{
    public class ProjectTag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Navigation properties
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }
} 