using SkillSwap.API.DTOs.Match;

namespace SkillSwap.API.Services.Interfaces
{
    
    public interface IMatchService
    {
       
        Task<MatchResultPageDto> GetMatchesForUserAsync(string userId, MatchFilterDto filter);

      
        Task<MatchResultPageDto> SearchListingsAsync(MatchFilterDto filter);

       
        Task<List<TrendingSkillDto>> GetTrendingSkillsAsync(int topN = 8);

      
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}