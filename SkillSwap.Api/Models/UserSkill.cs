using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.API.Models
{
    public enum SkillType
    {
        Offer,
        Need
    }
    
    public enum ProficiencyLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
    
    public class UserSkill
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int SkillId { get; set; }
        
        [Required]
        public SkillType SkillType { get; set; }
        
        public ProficiencyLevel? ProficiencyLevel { get; set; }
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
        [ForeignKey(nameof(SkillId))]
        public virtual Skill Skill { get; set; } = null!;

    }
}