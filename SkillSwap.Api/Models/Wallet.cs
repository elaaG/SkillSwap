using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.API.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Available balance cannot be negative")]
        public decimal AvailableBalance { get; set; } = 0m;
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Escrow balance cannot be negative")]
        public decimal EscrowBalance { get; set; } = 0m;
        
        [NotMapped]
        public decimal TotalBalance => AvailableBalance + EscrowBalance;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}