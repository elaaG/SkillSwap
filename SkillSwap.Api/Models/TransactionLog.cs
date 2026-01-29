using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.API.Models
{
    public enum TransactionType
    {
        InitialCredit,
        SessionPayment,
        EscrowHold,
        EscrowRelease,
        Refund,
        AdminAdjustment
    }
    
    public class TransactionLog
    {
        [Key]
        public int Id { get; set; }
        
        public string? FromUserId { get; set; }
        
        [Required]
        public string ToUserId { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public TransactionType Type { get; set; }
        
        public int? BookingId { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        public string TransactionReference { get; set; } = Guid.NewGuid().ToString();
        
        [ForeignKey("FromUserId")]
        public virtual ApplicationUser? FromUser { get; set; }
        
        [ForeignKey("ToUserId")]
        public virtual ApplicationUser ToUser { get; set; } = null!;
    }
}