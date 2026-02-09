using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.API.Models
{
    public enum ListingStatus
    {
        Active,
        Paused,
        Archived
    }

    public class Listing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProviderId { get; set; } = string.Empty;

        [ForeignKey(nameof(ProviderId))]
        public virtual ApplicationUser Provider { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(800)]
        public string? Description { get; set; }

        [Required]
        public int CreditsPerHour { get; set; } = 1;

        [Required]
        public ListingStatus Status { get; set; } = ListingStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<ListingSkill> ListingSkills { get; set; } = new List<ListingSkill>();
        public virtual ICollection<ListingAvailability> ListingAvailabilities { get; set; } = new List<ListingAvailability>();
    }
}