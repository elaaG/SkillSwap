using SkillSwap.API.DTOs.Wallet;

namespace SkillSwap.API.Services.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDto> GetWalletByUserIdAsync(string userId);
        Task<TransactionHistoryResponseDto> GetTransactionHistoryAsync(string userId, int page, int pageSize);
        Task<decimal> GetAvailableBalanceAsync(string userId);
        Task AddInitialCreditsAsync(string userId, decimal amount);
    }
}