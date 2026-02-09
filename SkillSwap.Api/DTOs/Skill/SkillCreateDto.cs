using System.ComponentModel.DataAnnotations;

namespace SkillSwap.API.DTOs.Skill
{
    public class SkillCreateDto
    {
        [Required, MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}