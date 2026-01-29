namespace SkillSwap.Client.Models
{
    public class UserProfile
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public decimal? AverageRating { get; set; }
        public int TotalSessionsCompleted { get; set; }
        public DateTime MemberSince { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}