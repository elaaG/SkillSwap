using System.ComponentModel.DataAnnotations;

namespace SkillSwap.API.DTOs.User
{
    public class UpdateProfileDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Bio { get; set; }
        
        [Url]
        [StringLength(500)]
        public string? ProfilePictureUrl { get; set; }
    }
}