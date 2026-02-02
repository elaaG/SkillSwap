using Microsoft.EntityFrameworkCore;
using SkillSwap.API.Data;
using SkillSwap.API.DTOs.Listing;
using SkillSwap.API.Helpers;
using SkillSwap.API.Models;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Services.Implementations
{
    public class ListingService : IListingService
    {
        private readonly ApplicationDbContext _db;

        public ListingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ListingResponseDto> CreateAsync(string providerId, ListingCreateDto dto)
        {
            ValidateAvailability(dto.ListingAvailabilities);

            var listing = new Listing
            {
                ProviderId = providerId,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                CreditsPerHour = dto.CreditsPerHour,
                Status = ListingStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _db.Listings.Add(listing);
            await _db.SaveChangesAsync();

            await ReplaceListingSkills(listing.Id, dto.Skills);
            await ReplaceAvailability(listing.Id, dto.ListingAvailabilities);

            return (await GetByIdAsync(listing.Id))!;
        }

        public async Task<ListingResponseDto?> GetByIdAsync(int id)
        {
            var listing = await _db.Listings
                .AsNoTracking()
                .Include(l => l.ListingSkills)
                    .ThenInclude(ls => ls.Skill)
                .Include(l => l.ListingAvailabilities )
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null) return null;

            return new ListingResponseDto
            {
                Id = listing.Id,
                ProviderId = listing.ProviderId,
                Title = listing.Title,
                Description = listing.Description,
                CreditsPerHour = listing.CreditsPerHour,
                Status = listing.Status.ToString(),
                CreatedAt = listing.CreatedAt,

                Skills = listing.ListingSkills
                    .OrderBy(x => x.Skill.Name)
                    .Select(x => new ListingSkillResponseDto
                    {
                        SkillId = x.SkillId,
                        SkillName = x.Skill.Name,
                        ProficiencyLevel = x.ProficiencyLevel
                    })
                    .ToList(),

                ListingAvailabilities = listing.ListingAvailabilities
                    .OrderBy(x => x.StartTime)
                    .Select(x => new ListingAvailabilityResponseDto
                    {
                        Id = x.Id,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime
                    })
                    .ToList()
            };
        }

        public async Task<PagedResult<ListingResponseDto>> GetAllAsync(
            string? search,
            int? skillId,
            string? status,
            int page,
            int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _db.Listings.AsNoTracking()
                .Include(l => l.ListingSkills).ThenInclude(ls => ls.Skill)
                .Include(l => l.ListingAvailabilities)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(l =>
                    l.Title.ToLower().Contains(s) ||
                    (l.Description != null && l.Description.ToLower().Contains(s)));
            }

            if (skillId.HasValue)
            {
                query = query.Where(l => l.ListingSkills.Any(ls => ls.SkillId == skillId.Value));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<ListingStatus>(status, true, out var parsed))
                {
                    query = query.Where(l => l.Status == parsed);
                }
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ListingResponseDto>
            {
                Items = items.Select(Map).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<ListingResponseDto?> UpdateAsync(int id, string providerId, ListingUpdateDto dto)
        {
            ValidateAvailability(dto.ListingAvailabilities);

            var listing = await _db.Listings
                .Include(l => l.ListingSkills)
                .Include(l => l.ListingAvailabilities)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null) return null;
            if (listing.ProviderId != providerId) throw new UnauthorizedAccessException("Not allowed.");

            listing.Title = dto.Title.Trim();
            listing.Description = dto.Description?.Trim();
            listing.CreditsPerHour = dto.CreditsPerHour;
            listing.Status = dto.Status;

            await _db.SaveChangesAsync();

            await ReplaceListingSkills(listing.Id, dto.Skills);
            await ReplaceAvailability(listing.Id, dto.ListingAvailabilities);

            return await GetByIdAsync(listing.Id);
        }

        public async Task<bool> DeleteAsync(int id, string providerId)
        {
            var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == id);
            if (listing == null) return false;

            if (listing.ProviderId != providerId)
                throw new UnauthorizedAccessException("Not allowed.");

            _db.Listings.Remove(listing);
            await _db.SaveChangesAsync();
            return true;
        }

        

        private async Task ReplaceListingSkills(int listingId, List<ListingSkillUpsertDto> skills)
        {
            // remove old
            var old = await _db.ListingSkills.Where(x => x.ListingId == listingId).ToListAsync();
            _db.ListingSkills.RemoveRange(old);
            await _db.SaveChangesAsync();

            if (skills == null || skills.Count == 0) return;

            // validate skill ids exist
            var ids = skills.Select(x => x.SkillId).Distinct().ToList();
            var existing = await _db.Skills.Where(s => ids.Contains(s.Id)).Select(s => s.Id).ToListAsync();

            if (existing.Count != ids.Count)
                throw new InvalidOperationException("One or more skills do not exist.");

            var newRows = skills
                .GroupBy(x => x.SkillId)
                .Select(g => new ListingSkill
                {
                    ListingId = listingId,
                    SkillId = g.Key,
                    ProficiencyLevel = g.First().ProficiencyLevel
                })
                .ToList();

            _db.ListingSkills.AddRange(newRows);
            await _db.SaveChangesAsync();
        }

        private async Task ReplaceAvailability(int listingId, List<ListingAvailabilityUpsertDto> slots)
        {
            var old = await _db.ListingAvailabilities.Where(x => x.ListingId == listingId).ToListAsync();
            _db.ListingAvailabilities.RemoveRange(old);
            await _db.SaveChangesAsync();

            if (slots == null || slots.Count == 0) return;

            var newRows = slots.Select(s => new ListingAvailability
            {
                ListingId = listingId,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }).ToList();

            _db.ListingAvailabilities.AddRange(newRows);
            await _db.SaveChangesAsync();
        }

        private static void ValidateAvailability(List<ListingAvailabilityUpsertDto> slots)
        {
            if (slots == null) return;

            foreach (var s in slots)
            {
                if (s.EndTime <= s.StartTime)
                    throw new InvalidOperationException("Availability end time must be after start time.");
            }
        }

        private static ListingResponseDto Map(Listing listing)
        {
            return new ListingResponseDto
            {
                Id = listing.Id,
                ProviderId = listing.ProviderId,
                Title = listing.Title,
                Description = listing.Description,
                CreditsPerHour = listing.CreditsPerHour,
                Status = listing.Status.ToString(),
                CreatedAt = listing.CreatedAt,

                Skills = listing.ListingSkills
                    .OrderBy(x => x.Skill.Name)
                    .Select(x => new ListingSkillResponseDto
                    {
                        SkillId = x.SkillId,
                        SkillName = x.Skill.Name,
                        ProficiencyLevel = x.ProficiencyLevel
                    })
                    .ToList(),

                ListingAvailabilities = listing.ListingAvailabilities
                    .OrderBy(x => x.StartTime)
                    .Select(x => new ListingAvailabilityResponseDto
                    {
                        Id = x.Id,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime
                    })
                    .ToList()
            };
        }
    }
}
