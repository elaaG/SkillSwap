using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillSwap.API.Data;
using SkillSwap.API.DTOs.Match;
using SkillSwap.API.Models;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Services.Implementations{
    
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MatchService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<MatchResultPageDto> GetMatchesForUserAsync(
            string userId,
            MatchFilterDto filter)
        {
            var needSkillIds = await _db.UserSkills
                .Where(us => us.UserId == userId && us.SkillType == SkillType.Need)
                .Select(us => us.SkillId)
                .ToListAsync();

            var query = BuildBaseQuery(excludeUserId: userId);
            query = ApplyFilters(query, filter);

            var rawItems = await ProjectToAnonymous(query);

            var scored = rawItems.Select(l =>
            {
                var listingSkillIds = l.Skills.Select(s => s.Id).ToHashSet();
                int overlap = needSkillIds.Intersect(listingSkillIds).Count();

                return MapToDto(l, relevanceScore: overlap, isPersonalMatch: overlap > 0);
            }).ToList();

            scored = ApplySorting(scored, filter.SortBy);

            return Paginate(scored, filter.Page, filter.PageSize);
        }

        public async Task<MatchResultPageDto> SearchListingsAsync(MatchFilterDto filter)
        {
            var query = BuildBaseQuery(excludeUserId: null);
            query = ApplyFilters(query, filter);

            var rawItems = await ProjectToAnonymous(query);

            var dtos = rawItems
                .Select(l => MapToDto(l, relevanceScore: 0, isPersonalMatch: false))
                .ToList();

            dtos = ApplySorting(dtos, filter.SortBy);
            return Paginate(dtos, filter.Page, filter.PageSize);
        }

        
        public async Task<List<TrendingSkillDto>> GetTrendingSkillsAsync(int topN = 8)
        {
            return await _db.ListingSkills
                .Where(ls => ls.Listing.Status == ListingStatus.Active)
                .GroupBy(ls => new { ls.SkillId, ls.Skill.Name })
                .Select(g => new TrendingSkillDto
                {
                    SkillId = g.Key.SkillId,
                    SkillName = g.Key.Name,
                    ActiveListingCount = g.Count()
                })
                .OrderByDescending(x => x.ActiveListingCount)
                .Take(topN)
                .ToListAsync();
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalListings = await _db.Listings
                .CountAsync(l => l.Status == ListingStatus.Active);

            var totalUsers = await _userManager.Users
                .CountAsync(u => u.IsActive);

            var totalSessions = await _userManager.Users
                .SumAsync(u => u.TotalSessionsCompleted);

            var totalSkills = await _db.Skills.CountAsync();

            return new DashboardStatsDto
            {
                TotalActiveListings = totalListings,
                TotalRegisteredUsers = totalUsers,
                TotalSessionsCompleted = totalSessions,
                TotalSkillsAvailable = totalSkills
            };
        }

        
        private IQueryable<Listing> BuildBaseQuery(string? excludeUserId)
        {
            IQueryable<Listing> q = _db.Listings
                .Where(l => l.Status == ListingStatus.Active)
                .Include(l => l.Provider)
                .Include(l => l.ListingSkills)
                    .ThenInclude(ls => ls.Skill)
                .Include(l => l.ListingAvailabilities)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(excludeUserId))
                q = q.Where(l => l.ProviderId != excludeUserId);

            return q;
        }

        
        private static IQueryable<Listing> ApplyFilters(
            IQueryable<Listing> q,
            MatchFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.ToLower();
                q = q.Where(l =>
                    l.Title.ToLower().Contains(term) ||
                    (l.Description != null && l.Description.ToLower().Contains(term)));
            }

            if (filter.SkillIds != null && filter.SkillIds.Count > 0)
            {
                foreach (var sid in filter.SkillIds)
                {
                    // capture loop variable to avoid closure issues
                    int capturedSid = sid;
                    q = q.Where(l => l.ListingSkills.Any(ls => ls.SkillId == capturedSid));
                }
            }

            if (filter.MinCredits.HasValue)
                q = q.Where(l => l.CreditsPerHour >= filter.MinCredits.Value);

            if (filter.MaxCredits.HasValue)
                q = q.Where(l => l.CreditsPerHour <= filter.MaxCredits.Value);

            // Minimum provider rating
            if (filter.MinRating.HasValue)
                q = q.Where(l =>
                    l.Provider.AverageRating.HasValue &&
                    l.Provider.AverageRating.Value >= filter.MinRating.Value);

            return q;
        }

        private record RawListing(
            int Id,
            string Title,
            string? Description,
            int CreditsPerHour,
            ListingStatus Status,
            DateTime CreatedAt,
            string ProviderId,
            string ProviderName,
            string? ProviderAvatarUrl,
            decimal? ProviderRating,
            int ProviderSessions,
            List<(int Id, string Name, string? ProfLevel)> Skills,
            DateTime? NextSlot);

        
        private static async Task<List<RawListing>> ProjectToAnonymous(IQueryable<Listing> q)
        {
            var rows = await q.Select(l => new
            {
                l.Id,
                l.Title,
                l.Description,
                l.CreditsPerHour,
                l.Status,
                l.CreatedAt,
                l.ProviderId,
                ProviderName = l.Provider.FullName,
                ProviderAvatarUrl = l.Provider.ProfilePictureUrl,
                ProviderRating = l.Provider.AverageRating,
                ProviderSessions = l.Provider.TotalSessionsCompleted,
                Skills = l.ListingSkills.Select(ls => new
                {
                    ls.Skill.Id,
                    ls.Skill.Name,
                    ProfLevel = ls.ProficiencyLevel != null
                        ? ls.ProficiencyLevel.ToString()
                        : (string?)null
                }).ToList(),
                NextSlot = l.ListingAvailabilities
                    .Where(a => !a.IsBooked && a.StartTime > DateTime.UtcNow)
                    .OrderBy(a => a.StartTime)
                    .Select(a => (DateTime?)a.StartTime)
                    .FirstOrDefault()
            }).ToListAsync();

            return rows.Select(r => new RawListing(
                r.Id, r.Title, r.Description, r.CreditsPerHour, r.Status, r.CreatedAt,
                r.ProviderId, r.ProviderName, r.ProviderAvatarUrl, r.ProviderRating,
                r.ProviderSessions,
                r.Skills.Select(s => (s.Id, s.Name, s.ProfLevel)).ToList(),
                r.NextSlot
            )).ToList();
        }

        private static MatchedListingDto MapToDto(
            RawListing l,
            int relevanceScore,
            bool isPersonalMatch)
        {
            return new MatchedListingDto
            {
                ListingId = l.Id,
                Title = l.Title,
                Description = l.Description,
                CreditsPerHour = l.CreditsPerHour,
                Status = l.Status.ToString(),
                CreatedAt = l.CreatedAt,
                ProviderId = l.ProviderId,
                ProviderName = l.ProviderName,
                ProviderAvatarUrl = l.ProviderAvatarUrl,
                ProviderRating = l.ProviderRating,
                ProviderSessionsCompleted = l.ProviderSessions,
                Skills = l.Skills.Select(s => new SkillTagDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ProficiencyLevel = s.ProfLevel
                }).ToList(),
                NextAvailableSlot = l.NextSlot,
                RelevanceScore = relevanceScore,
                IsPersonalMatch = isPersonalMatch
            };
        }

        
        private static List<MatchedListingDto> ApplySorting(
            List<MatchedListingDto> items,
            string sortBy)
        {
            return sortBy.ToLowerInvariant() switch
            {
                "rating"  => items.OrderByDescending(x => x.ProviderRating ?? 0m).ToList(),
                "credits" => items.OrderBy(x => x.CreditsPerHour).ToList(),
                "newest"  => items.OrderByDescending(x => x.CreatedAt).ToList(),
                _         => items
                                .OrderByDescending(x => x.RelevanceScore)
                                .ThenByDescending(x => x.ProviderRating ?? 0m)
                                .ToList()
            };
        }

        private static MatchResultPageDto Paginate(
            List<MatchedListingDto> items,
            int page,
            int pageSize)
        {
            int total = items.Count;
            var paged = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new MatchResultPageDto
            {
                Items = paged,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}