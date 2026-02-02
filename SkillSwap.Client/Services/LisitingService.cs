using SkillSwap.Client.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SkillSwap.Client.Services
{
    public interface IListingService
    {
        Task<ApiResponse<List<Listing>>> GetAllListingsAsync();
        Task<ApiResponse<Listing>> GetListingByIdAsync(int id);
        Task<ApiResponse<Listing>> CreateListingAsync(ListingCreateDto dto);
    }

    public class ListingService : IListingService
    {
        private readonly HttpClient _httpClient;

        public ListingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<List<Listing>>> GetAllListingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/listings");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var pagedResult = JsonSerializer.Deserialize<PagedResult<Listing>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (pagedResult?.Items != null)
                        {
                            return new ApiResponse<List<Listing>> 
                            { 
                                Success = true, 
                                Data = pagedResult.Items 
                            };
                        }
                    }
                    catch
                    {
                        var listings = JsonSerializer.Deserialize<List<Listing>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        return new ApiResponse<List<Listing>> 
                        { 
                            Success = true, 
                            Data = listings ?? new List<Listing>() 
                        };
                    } 
                }
                
                return new ApiResponse<List<Listing>> 
                { 
                    Success = false, 
                    ErrorMessage = $"Failed to load listings: {response.StatusCode}" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Listing>>
                {
                    Success = false, 
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<Listing>> GetListingByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/listings/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var listing = await response.Content.ReadFromJsonAsync<Listing>();
                    return new ApiResponse<Listing>
                    {
                        Success = true,
                        Data = listing
                    };
                }
                return new ApiResponse<Listing> 
                { 
                    Success = false, 
                    ErrorMessage = $"Listing not found: {response.StatusCode}" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Listing>
                {
                    Success = false, 
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<Listing>> CreateListingAsync(ListingCreateDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/listings", dto);
                if (response.IsSuccessStatusCode)
                {
                    var listing = await response.Content.ReadFromJsonAsync<Listing>();
                    return new ApiResponse<Listing> { Success = true, Data = listing };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Listing>
                {
                    Success = false, 
                    ErrorMessage = error
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Listing>
                {
                    Success = false, 
                    ErrorMessage = ex.Message
                };
            }
        }
    }
    // Helper class to deserialize paged results
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
