namespace SkillSwap.Client.Models
{
    public class Listing
    {
        public int Id { get; set; }
        public string ProviderId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreditsPerHour { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }

        public List<ListingSkill> Skills { get; set; } = new();
        public List<ListingAvailability> ListingAvailabilities { get; set; } = new();
    }

    public class ListingSkill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? ProficiencyLevel { get; set; }
    }

    public class ListingAvailability
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class ListingCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreditsPerHour { get; set; } = 1;
        public List<ListingSkillUpsertDto> Skills { get; set; } = new();
        public List<ListingAvailabilityUpsertDto> ListingAvailabilities { get; set; } = new();
    }

    public class ListingSkillUpsertDto
    {
        public int SkillId { get; set; }
        public string? ProficiencyLevel { get; set; }
    }

    public class ListingAvailabilityUpsertDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}