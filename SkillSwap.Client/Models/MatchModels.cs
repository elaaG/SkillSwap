namespace SkillSwap.Client.Models
{
    public class MatchFilterDto
    {
        public string? Search { get; set; }
        public List<int>? SkillIds { get; set; }
        public int? MinCredits { get; set; }
        public int? MaxCredits { get; set; }
        public decimal? MinRating { get; set; }
        public string SortBy { get; set; } = "relevance";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class MatchResultPageDto
    {
        public List<MatchedListingDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class MatchedListingDto
    {
        public int ListingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreditsPerHour { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string ProviderId { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string? ProviderAvatarUrl { get; set; }
        public decimal? ProviderRating { get; set; }
        public int ProviderSessionsCompleted { get; set; }

        public List<SkillTagDto> Skills { get; set; } = new();

        public DateTime? NextAvailableSlot { get; set; }
        public int RelevanceScore { get; set; }
        public bool IsPersonalMatch { get; set; }
    }

    public class SkillTagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProficiencyLevel { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalActiveListings { get; set; }
        public int TotalRegisteredUsers { get; set; }
        public int TotalSessionsCompleted { get; set; }
        public int TotalSkillsAvailable { get; set; }
    }

    public class TrendingSkillDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int ActiveListingCount { get; set; }
    }
}
