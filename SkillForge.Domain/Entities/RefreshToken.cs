using SkillForge.Domain.Common;

namespace SkillForge.Domain.Entities
{
    public class RefreshToken : AuditableEntity
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
} 