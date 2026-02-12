using SkillSwap.Client.Models;
using System.Net.Http.Json;

namespace SkillSwap.Client.Services
{
    public interface IMatchService
    {
        Task<ApiResponse<MatchResultPageDto>> GetMatchesForMeAsync(MatchFilterDto filter);
        Task<ApiResponse<MatchResultPageDto>> SearchAsync(MatchFilterDto filter);
        Task<ApiResponse<List<TrendingSkillDto>>> GetTrendingAsync(int topN = 8);
        Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
    }

    public class MatchService : IMatchService
    {
        private readonly HttpClient _httpClient;

        public MatchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<MatchResultPageDto>> GetMatchesForMeAsync(MatchFilterDto filter)
        {
            try
            {
                var query = BuildQueryString(filter);
                var result = await _httpClient.GetFromJsonAsync<MatchResultPageDto>($"api/match/for-me{query}");

                if (result != null)
                {
                    return new ApiResponse<MatchResultPageDto>
                    {
                        Success = true,
                        Data = result
                    };
                }

                return new ApiResponse<MatchResultPageDto>
                {
                    Success = false,
                    ErrorMessage = "Failed to load personalized matches"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MatchResultPageDto>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<MatchResultPageDto>> SearchAsync(MatchFilterDto filter)
        {
            try
            {
                var query = BuildQueryString(filter);
                var result = await _httpClient.GetFromJsonAsync<MatchResultPageDto>($"api/match/search{query}");

                if (result != null)
                {
                    return new ApiResponse<MatchResultPageDto>
                    {
                        Success = true,
                        Data = result
                    };
                }

                return new ApiResponse<MatchResultPageDto>
                {
                    Success = false,
                    ErrorMessage = "Failed to load search results"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MatchResultPageDto>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<TrendingSkillDto>>> GetTrendingAsync(int topN = 8)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<TrendingSkillDto>>($"api/match/trending?topN={topN}");

                if (result != null)
                {
                    return new ApiResponse<List<TrendingSkillDto>>
                    {
                        Success = true,
                        Data = result
                    };
                }

                return new ApiResponse<List<TrendingSkillDto>>
                {
                    Success = false,
                    ErrorMessage = "Failed to load trending skills"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TrendingSkillDto>>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<DashboardStatsDto>("api/match/dashboard-stats");

                if (result != null)
                {
                    return new ApiResponse<DashboardStatsDto>
                    {
                        Success = true,
                        Data = result
                    };
                }

                return new ApiResponse<DashboardStatsDto>
                {
                    Success = false,
                    ErrorMessage = "Failed to load dashboard stats"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<DashboardStatsDto>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private string BuildQueryString(MatchFilterDto filter)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(filter.Search))
                queryParams.Add($"Search={Uri.EscapeDataString(filter.Search)}");

            if (filter.SkillIds != null && filter.SkillIds.Any())
            {
                foreach (var skillId in filter.SkillIds)
                    queryParams.Add($"SkillIds={skillId}");
            }

            if (filter.MinCredits.HasValue)
                queryParams.Add($"MinCredits={filter.MinCredits.Value}");

            if (filter.MaxCredits.HasValue)
                queryParams.Add($"MaxCredits={filter.MaxCredits.Value}");

            if (filter.MinRating.HasValue)
                queryParams.Add($"MinRating={filter.MinRating.Value}");

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
                queryParams.Add($"SortBy={filter.SortBy}");

            queryParams.Add($"Page={filter.Page}");
            queryParams.Add($"PageSize={filter.PageSize}");

            return queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
        }
    }
}
