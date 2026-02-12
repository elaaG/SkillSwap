using SkillSwap.Api.Models;

namespace SkillSwap.Api.DTOs.Booking;

public class BookingResponseDto
{
    public Guid BookingId { get; set; }
    public BookingState State { get; set; }
    public EscrowStatus EscrowStatus { get; set; }
}