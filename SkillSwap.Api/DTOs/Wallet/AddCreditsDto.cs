using System.ComponentModel.DataAnnotations;

namespace SkillSwap.API.DTOs.Wallet
{
    public class AddCreditsDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
