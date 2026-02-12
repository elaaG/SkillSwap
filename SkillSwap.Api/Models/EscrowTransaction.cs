namespace SkillSwap.Api.Models;

public class EscrowTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public EscrowStatus Status { get; set; } = EscrowStatus.Hold;
    //  REAL FOREIGN KEY (must match Booking.Id type)
    public Guid BookingId { get; set; }
    //  Navigation property (required for 1–1)
    public Booking Booking { get; set; } = null!;

}