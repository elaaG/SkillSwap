namespace SkillSwap.API.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal EscrowBalance { get; set; }
    }
}