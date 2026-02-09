namespace SkillSwap.Api.Models;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ClientId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid ListingId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public BookingState State { get; set; } = BookingState.Pending;

    public EscrowTransaction Escrow { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}