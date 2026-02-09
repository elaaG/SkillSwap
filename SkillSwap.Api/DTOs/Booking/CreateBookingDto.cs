namespace SkillSwap.Api.DTOs.Booking;

public class CreateBookingDto
{
    public Guid ListingId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
}