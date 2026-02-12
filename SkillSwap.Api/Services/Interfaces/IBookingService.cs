using SkillSwap.Api.DTOs.Booking;
using SkillSwap.Api.Models;

namespace SkillSwap.Api.Services.Interfaces;

public interface IBookingService
{
    // old ACTIONS for SERVICE
    // Booking Create(Guid clientId, CreateBookingDto dto);
    // void Accept(Booking booking);
    // void Complete(Booking booking);
    // void Reject(Booking booking);
    
    Task<BookingResponseDto> CreateAsync(string clientId, CreateBookingDto dto);
    Task AcceptAsync(Guid bookingId);

    Task CompleteAsync(Guid bookingId);

    Task RejectAsync(Guid bookingId);
    Task<List<BookingResponseDto>> GetMyBookingsAsync(string userId);

}