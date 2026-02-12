using SkillSwap.Api.Models;

namespace SkillSwap.Api.DTOs.Booking;

public class BookingResponseDto
{
    public Guid BookingId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;

    public int ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public BookingState State { get; set; }
    public EscrowStatus EscrowStatus { get; set; }
}