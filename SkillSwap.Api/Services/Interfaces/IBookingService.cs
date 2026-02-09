using SkillSwap.Api.DTOs.Booking;
using SkillSwap.Api.Models;

namespace SkillSwap.Api.Services.Interfaces;

public interface IBookingService
{
    Booking Create(Guid clientId, CreateBookingDto dto);
    void Accept(Booking booking);
    void Complete(Booking booking);
    void Reject(Booking booking);
}