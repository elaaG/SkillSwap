namespace SkillSwap.API.DTOs.Wallet
{
    public class WalletDto
    {
        public int WalletId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal AvailableBalance { get; set; }
        public decimal EscrowBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}