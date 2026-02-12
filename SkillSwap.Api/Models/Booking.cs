using SkillSwap.API.Models;

namespace SkillSwap.Api.Models;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ClientId { get; set; }
    public string ProviderId { get; set; }
    public int ListingId { get; set; }
    public Listing Listing { get; set; } = null!; // ✅ NAVIGATION
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public BookingState State { get; set; } = BookingState.Pending;

    public EscrowTransaction? Escrow { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}