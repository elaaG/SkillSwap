using System.ComponentModel.DataAnnotations;

namespace SkillSwap.API.DTOs.Skill
{
    public class SkillResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}