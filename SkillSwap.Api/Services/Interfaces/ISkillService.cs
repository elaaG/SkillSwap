using SkillSwap.API.DTOs.Skill;
using SkillSwap.API.Helpers;

namespace SkillSwap.API.Services.Interfaces
{
    public interface ISkillService
    {
        Task<SkillResponseDto> CreateAsync(SkillCreateDto dto);
        Task<SkillResponseDto?> GetByIdAsync(int id);
        Task<PagedResult<SkillResponseDto>> GetAllAsync(string? search, int page, int pageSize);
        Task<SkillResponseDto?> UpdateAsync(int id, SkillUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}