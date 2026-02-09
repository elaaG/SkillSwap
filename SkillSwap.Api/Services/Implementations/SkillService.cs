using Microsoft.EntityFrameworkCore;
using SkillSwap.API.Data;
using SkillSwap.API.DTOs.Skill;
using SkillSwap.API.Helpers;
using SkillSwap.API.Models;
using SkillSwap.API.Services.Interfaces;


namespace SkillSwap.API.Services.Implementations
{
    public class SkillService : ISkillService
    {
        private readonly ApplicationDbContext _db;

        public SkillService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<SkillResponseDto> CreateAsync(SkillCreateDto dto)
        {
            var name = dto.Name.Trim();

            var exists = await _db.Skills.AnyAsync(s => s.Name.ToLower() == name.ToLower());
            if (exists)
                throw new InvalidOperationException("Skill already exists.");

            var skill = new Skill
            {
                Name = name,
                Description = dto.Description?.Trim()
            };

            _db.Skills.Add(skill);
            await _db.SaveChangesAsync();

            return Map(skill);
        }

        public async Task<SkillResponseDto?> GetByIdAsync(int id)
        {
            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == id);
            return skill == null ? null : Map(skill);
        }

        public async Task<PagedResult<SkillResponseDto>> GetAllAsync(string? search, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _db.Skills.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(s));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SkillResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<SkillResponseDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<SkillResponseDto?> UpdateAsync(int id, SkillUpdateDto dto)
        {
            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null) return null;

            var name = dto.Name.Trim();

            var exists = await _db.Skills.AnyAsync(s => s.Id != id && s.Name.ToLower() == name.ToLower());
            if (exists)
                throw new InvalidOperationException("Another skill with same name already exists.");

            skill.Name = name;
            skill.Description = dto.Description?.Trim();

            await _db.SaveChangesAsync();
            return Map(skill);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null) return false;

            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();
            return true;
        }

        private static SkillResponseDto Map(Skill s) => new SkillResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            CreatedAt = s.CreatedAt
        };
    }
}
