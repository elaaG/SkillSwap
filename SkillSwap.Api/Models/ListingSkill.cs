using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.API.Models
{
    public class ListingSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ListingId { get; set; }

        [ForeignKey(nameof(ListingId))]
        public virtual Listing Listing { get; set; } = null!;

        [Required]
        public int SkillId { get; set; }

        [ForeignKey(nameof(SkillId))]
        public virtual Skill Skill { get; set; } = null!;

        public ProficiencyLevel? ProficiencyLevel { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}