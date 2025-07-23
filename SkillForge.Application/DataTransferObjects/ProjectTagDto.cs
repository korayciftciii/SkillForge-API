using System;

namespace SkillForge.Application.DataTransferObjects
{
    public class ProjectTagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        
        // Optional project information
        public string? ProjectTitle { get; set; }
    }
} 