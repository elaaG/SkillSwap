using SkillSwap.API.DTOs.Listing;
using SkillSwap.API.Helpers;

namespace SkillSwap.API.Services.Interfaces
{
    public interface IListingService
    {
        Task<ListingResponseDto> CreateAsync(string providerId, ListingCreateDto dto);
        Task<ListingResponseDto?> GetByIdAsync(int id);
        Task<PagedResult<ListingResponseDto>> GetAllAsync(
            string? search,
            int? skillId,
            string? status,
            int page,
            int pageSize);

        Task<ListingResponseDto?> UpdateAsync(int id, string providerId, ListingUpdateDto dto);
        Task<bool> DeleteAsync(int id, string providerId);
    }
}