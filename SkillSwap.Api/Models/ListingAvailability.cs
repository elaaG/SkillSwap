using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.API.Models
{
    public class ListingAvailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ListingId { get; set; }

        [ForeignKey(nameof(ListingId))]
        public virtual Listing Listing { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public bool IsBooked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}