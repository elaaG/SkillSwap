namespace SkillSwap.Client.Models
{
    // public class BookingDto
    // {
    //     public Guid BookingId { get; set; }
    //     public string State { get; set; } = string.Empty;
    //     public string EscrowStatus { get; set; } = string.Empty;
    // }
    public class BookingDto
    {
        public Guid BookingId { get; set; }

        // 🔑 identity
        public string ClientId { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty;

        // 🔁 state
        public string State { get; set; } = string.Empty;
        public string EscrowStatus { get; set; } = string.Empty;

        // (optional but useful)
        public int ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
    }

    public class CreateBookingDto
    {
        public string ProviderId { get; set; } = string.Empty;
        public int ListingId { get; set; }
        // public DateTime StartTime { get; set; }
        // public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public int AvailabilityId { get; set; }

    }
}
