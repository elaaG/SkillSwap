using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillSwap.API.Data;
using SkillSwap.API.DTOs.User;
using SkillSwap.API.Exceptions;
using SkillSwap.API.Models;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;
        
        public UserService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }
        
        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
            {
                throw new NotFoundException($"User {userId} not found");
            }
            
            return new UserProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                AverageRating = user.AverageRating,
                TotalSessionsCompleted = user.TotalSessionsCompleted,
                MemberSince = user.CreatedAt
            };
        }
        
        public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                throw new NotFoundException($"User {userId} not found");
            }
            
            user.FullName = updateDto.FullName;
            user.Bio = updateDto.Bio;
            user.ProfilePictureUrl = updateDto.ProfilePictureUrl;
            
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Profile update failed: {errors}");
            }
            
            _logger.LogInformation("User {UserId} updated their profile", userId);
            
            return new UserProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                AverageRating = user.AverageRating,
                TotalSessionsCompleted = user.TotalSessionsCompleted,
                MemberSince = user.CreatedAt
            };
        }
        
        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _userManager.Users.AnyAsync(u => u.Id == userId);
        }
    }
}