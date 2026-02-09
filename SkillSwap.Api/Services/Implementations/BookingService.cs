using SkillSwap.Api.DTOs.Booking;
using SkillSwap.Api.Models;
using SkillSwap.Api.Services.Interfaces;

namespace SkillSwap.Api.Services.Implementations;

public class BookingService : IBookingService
{
    public Booking Create(Guid clientId, CreateBookingDto dto)
    {
        if (dto.EndTime <= dto.StartTime)
            throw new Exception("Invalid time slot");

        var booking = new Booking
        {
            ClientId = clientId,
            ProviderId = dto.ProviderId,
            ListingId = dto.ListingId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Escrow = new EscrowTransaction
            {
                Amount = dto.Price,
                Status = EscrowStatus.Hold
            }
        };

        return booking;
    }

    public void Accept(Booking booking)
    {
        if (booking.State != BookingState.Pending)
            throw new Exception("Only pending bookings can be accepted");

        booking.State = BookingState.Accepted;
    }

    public void Complete(Booking booking)
    {
        if (booking.State != BookingState.Accepted)
            throw new Exception("Only accepted bookings can be completed");

        booking.State = BookingState.Completed;
        booking.Escrow.Status = EscrowStatus.Released;
    }

    public void Reject(Booking booking)
    {
        if (booking.State != BookingState.Pending)
            throw new Exception("Only pending bookings can be rejected");

        booking.State = BookingState.Rejected;
        booking.Escrow.Status = EscrowStatus.Refunded;
    }
}