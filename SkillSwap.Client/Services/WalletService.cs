using Blazored.LocalStorage;
using SkillSwap.Client.Models;
using System.Net.Http.Json;

namespace SkillSwap.Client.Services
{
    public interface IWalletService
    {
        Task<ApiResponse<Wallet>> GetWalletAsync();
        Task<ApiResponse<TransactionHistoryResponse>> GetTransactionHistoryAsync(int page = 1, int pageSize = 20);
    }

    public class WalletService : IWalletService
    {
        private readonly HttpClient _httpClient;

        public WalletService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<Wallet>> GetWalletAsync()
        {
            try
            {
                var wallet = await _httpClient.GetFromJsonAsync<Wallet>("api/wallet");
                
                if (wallet != null)
                {
                    return new ApiResponse<Wallet> { Success = true, Data = wallet };
                }

                return new ApiResponse<Wallet>
                {
                    Success = false,
                    ErrorMessage = "Failed to load wallet"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Wallet>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<TransactionHistoryResponse>> GetTransactionHistoryAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                var history = await _httpClient.GetFromJsonAsync<TransactionHistoryResponse>(
                    $"api/wallet/transactions?page={page}&pageSize={pageSize}");

                if (history != null)
                {
                    return new ApiResponse<TransactionHistoryResponse> { Success = true, Data = history };
                }

                return new ApiResponse<TransactionHistoryResponse>
                {
                    Success = false,
                    ErrorMessage = "Failed to load transaction history"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TransactionHistoryResponse>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}