using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Api.DTOs.Booking;
using SkillSwap.Api.Services.Interfaces;

namespace SkillSwap.API.Controllers;
[ApiController]
[Route("api/bookings")]
public class BookingController:ControllerBase
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized("User not authenticated");
        }
        // var clientId = User.FindFirst("sub")!.Value;
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(clientId))
        {
            return Unauthorized("User ID missing from token");
        }
        try
        {
            var booking = await _service.CreateAsync(clientId, dto);
            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // optional logging
            Console.WriteLine(ex);
            return StatusCode(500, "An unexpected error occurred");
        }
    }
    [HttpPost("{id}/accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        await _service.AcceptAsync(id);
        return Ok();
    }
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await _service.CompleteAsync(id);
        return Ok();
    }
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        await _service.RejectAsync(id);
        return Ok();
    }
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var bookings = await _service.GetMyBookingsAsync(userId);

        return Ok(bookings);
    }


}