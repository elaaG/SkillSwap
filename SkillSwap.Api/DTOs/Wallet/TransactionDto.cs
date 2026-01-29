namespace SkillSwap.API.DTOs.Wallet
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty; // "Sent" or "Received"
        public string? PartnerName { get; set; }
        public string? PartnerId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }
    }
}