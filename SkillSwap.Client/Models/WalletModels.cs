namespace SkillSwap.Client.Models
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal AvailableBalance { get; set; }
        public decimal EscrowBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string? PartnerName { get; set; }
        public string? PartnerId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }
    }

    public class TransactionHistoryResponse
    {
        public List<Transaction> Transactions { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}