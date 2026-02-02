using System.ComponentModel.DataAnnotations;
using SkillSwap.API.Models;

namespace SkillSwap.API.DTOs.Listing
{
    public class ListingSkillUpsertDto
    {
        [Required]
        public int SkillId { get; set; }

        public ProficiencyLevel? ProficiencyLevel { get; set; }
    }

    public class ListingAvailabilityUpsertDto
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }

    public class ListingCreateDto
    {
        [Required, MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(800)]
        public string? Description { get; set; }

        [Range(1, 999999)]
        public int CreditsPerHour { get; set; } = 1;

        public List<ListingSkillUpsertDto> Skills { get; set; } = new();
        public List<ListingAvailabilityUpsertDto> ListingAvailabilities { get; set; } = new();
    }
    public class ListingUpdateDto
    {
        [Required, MaxLength(120)]
        public string? Title { get; set; } 

        [MaxLength(800)]
        public string? Description { get; set; }

        [Range(1, 999999)]
        public int CreditsPerHour { get; set; } = 1;

        public ListingStatus Status { get; set; } = ListingStatus.Active;

        public List<ListingSkillUpsertDto> Skills { get; set; } = new();
        public List<ListingAvailabilityUpsertDto> ListingAvailabilities { get; set; } = new();
    }
    public class ListingSkillResponseDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public ProficiencyLevel? ProficiencyLevel { get; set; }
    }

    public class ListingAvailabilityResponseDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class ListingResponseDto
    {
        public int Id { get; set; }
        public string ProviderId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreditsPerHour { get; set; }

        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }

        public List<ListingSkillResponseDto> Skills { get; set; } = new();
        public List<ListingAvailabilityResponseDto> ListingAvailabilities { get; set; } = new();
    }
}