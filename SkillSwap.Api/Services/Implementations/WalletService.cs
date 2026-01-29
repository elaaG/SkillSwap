using Microsoft.EntityFrameworkCore;
using SkillSwap.API.Data;
using SkillSwap.API.DTOs.Wallet;
using SkillSwap.API.Exceptions;
using SkillSwap.API.Models;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WalletService> _logger;
        
        public WalletService(ApplicationDbContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<WalletDto> GetWalletByUserIdAsync(string userId)
        {
            var wallet = await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId);
            
            if (wallet == null)
            {
                throw new NotFoundException($"Wallet not found for user {userId}");
            }
            
            return new WalletDto
            {
                WalletId = wallet.Id,
                UserId = wallet.UserId,
                AvailableBalance = wallet.AvailableBalance,
                EscrowBalance = wallet.EscrowBalance,
                TotalBalance = wallet.TotalBalance,
                LastUpdated = wallet.UpdatedAt
            };
        }
        
        public async Task<TransactionHistoryResponseDto> GetTransactionHistoryAsync(
            string userId, 
            int page, 
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;
            
            var query = _context.TransactionLogs
                .AsNoTracking()
                .Where(t => t.FromUserId == userId || t.ToUserId == userId)
                .OrderByDescending(t => t.Timestamp);
            
            var totalCount = await query.CountAsync();
            
            var transactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.FromUser)
                .Include(t => t.ToUser)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    TransactionReference = t.TransactionReference,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    Direction = t.FromUserId == userId ? "Sent" : "Received",
                    PartnerName = t.FromUserId == userId 
                        ? t.ToUser.FullName 
                        : (t.FromUser != null ? t.FromUser.FullName : "System"),
                    PartnerId = t.FromUserId == userId ? t.ToUserId : t.FromUserId,
                    Timestamp = t.Timestamp,
                    Notes = t.Notes
                })
                .ToListAsync();
            
            return new TransactionHistoryResponseDto
            {
                Transactions = transactions,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        
        public async Task<decimal> GetAvailableBalanceAsync(string userId)
        {
            var wallet = await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId);
            
            if (wallet == null)
            {
                throw new NotFoundException($"Wallet not found for user {userId}");
            }
            
            return wallet.AvailableBalance;
        }
        
        public async Task AddInitialCreditsAsync(string userId, decimal amount)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
            
            if (wallet == null)
            {
                throw new NotFoundException($"Wallet not found for user {userId}");
            }
            
            wallet.AvailableBalance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            
            var transactionLog = new TransactionLog
            {
                ToUserId = userId,
                Amount = amount,
                Type = TransactionType.InitialCredit,
                Timestamp = DateTime.UtcNow,
                Notes = $"Initial credits added: {amount}",
                TransactionReference = Guid.NewGuid().ToString()
            };
            
            _context.TransactionLogs.Add(transactionLog);
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Added {Amount} initial credits to user {UserId}", amount, userId);
        }
    }
}