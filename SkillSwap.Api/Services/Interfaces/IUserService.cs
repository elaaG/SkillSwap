using SkillSwap.API.DTOs.User;

namespace SkillSwap.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(string userId);
        Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto updateDto);
        Task<bool> UserExistsAsync(string userId);
    }
}