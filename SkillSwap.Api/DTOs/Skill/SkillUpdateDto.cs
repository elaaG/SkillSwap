using System.ComponentModel.DataAnnotations;

namespace SkillSwap.API.DTOs.Skill
{
    public class SkillUpdateDto
    {
        [Required, MaxLength(80)]
        public string? Name { get; set; } 

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}