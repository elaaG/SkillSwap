using Microsoft.AspNetCore.Identity;

namespace SkillSwap.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public decimal? AverageRating { get; set; }
        public int TotalSessionsCompleted { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        
        public virtual Wallet? Wallet { get; set; }
        public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }
}