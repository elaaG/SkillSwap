using SkillSwap.API.DTOs.Auth;

namespace SkillSwap.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<string> GenerateJwtToken(string userId, string email, string fullName);
    }
}