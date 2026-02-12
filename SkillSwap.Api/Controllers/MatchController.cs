using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.API.DTOs.Match;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Controllers
{
    
    [ApiController]
    [Route("api/match")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ILogger<MatchController> _logger;

        public MatchController(IMatchService matchService, ILogger<MatchController> logger)
        {
            _matchService = matchService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("for-me")]
        [ProducesResponseType(typeof(MatchResultPageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MatchResultPageDto>> GetMatchesForMe(
            [FromQuery] MatchFilterDto filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            _logger.LogInformation(
                "Personalised match request – userId={UserId} page={Page} sortBy={SortBy}",
                userId, filter.Page, filter.SortBy);

            var result = await _matchService.GetMatchesForUserAsync(userId, filter);
            return Ok(result);
        }

       
        [AllowAnonymous]
        [HttpGet("search")]
        [ProducesResponseType(typeof(MatchResultPageDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<MatchResultPageDto>> Search(
            [FromQuery] MatchFilterDto filter)
        {
            _logger.LogInformation(
                "Public search – term='{Term}' skillIds={Skills} page={Page}",
                filter.Search, filter.SkillIds, filter.Page);

            var result = await _matchService.SearchListingsAsync(filter);
            return Ok(result);
        }

        
        [AllowAnonymous]
        [HttpGet("trending")]
        [ProducesResponseType(typeof(List<TrendingSkillDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TrendingSkillDto>>> GetTrending(
            [FromQuery] int topN = 8)
        {
            var result = await _matchService.GetTrendingSkillsAsync(topN);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("dashboard-stats")]
        [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var stats = await _matchService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}