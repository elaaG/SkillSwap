namespace SkillSwap.Api.DTOs.Booking;

public class CreateBookingDto
{
    public int ListingId { get; set; }
    public string ProviderId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
}