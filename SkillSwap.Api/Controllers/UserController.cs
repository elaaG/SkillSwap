using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.API.DTOs.User;
using SkillSwap.API.Helpers;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
       
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = User.GetUserId();
            var profile = await _userService.GetProfileAsync(userId);
            
            return Ok(profile);
        }
        
        
        [HttpGet("profile/{userId}")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileDto>> GetProfileById(string userId)
        {
            var profile = await _userService.GetProfileAsync(userId);
            
            return Ok(profile);
        }
       
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileDto updateDto)
        {
            var userId = User.GetUserId();
            var updatedProfile = await _userService.UpdateProfileAsync(userId, updateDto);
            
            return Ok(updatedProfile);
        }
    }
}