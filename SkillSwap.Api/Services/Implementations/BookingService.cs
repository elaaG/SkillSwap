using SkillSwap.Api.DTOs.Booking;
using SkillSwap.Api.Models;
using SkillSwap.API.Models;
using SkillSwap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SkillSwap.API.Data;


namespace SkillSwap.Api.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    public BookingService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<BookingResponseDto> CreateAsync(string clientId, CreateBookingDto dto)
    {
        // Get or create client wallet
        var clientWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == clientId);
        
        if (clientWallet == null)
        {
            // Create wallet automatically if it doesn't exist
            clientWallet = new Wallet
            {
                UserId = clientId,
                AvailableBalance = 0,
                EscrowBalance = 0
            };
            _context.Wallets.Add(clientWallet);
            await _context.SaveChangesAsync();
        }

        if (clientWallet.AvailableBalance < dto.Price)
            throw new Exception("Not enough balance");

        // Get or create provider wallet
        var providerWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == dto.ProviderId);
        if (providerWallet == null)
        {
            providerWallet = new Wallet
            {
                UserId = dto.ProviderId,
                AvailableBalance = 0,
                EscrowBalance = 0
            };
            _context.Wallets.Add(providerWallet);
            await _context.SaveChangesAsync();
        }

        // Move money into escrow
        clientWallet.AvailableBalance -= dto.Price;
        clientWallet.EscrowBalance += dto.Price;

        var booking = new Booking
        {
            ClientId = clientId,
            ProviderId = dto.ProviderId,
            ListingId = dto.ListingId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            State = BookingState.Pending, 
            Escrow = new EscrowTransaction
            {
                Amount = dto.Price,
                Status = EscrowStatus.Hold
            }
        };

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();
        return new BookingResponseDto
        {
            BookingId = booking.Id,
            State = booking.State,
            EscrowStatus = booking.Escrow.Status
        };
    }
    
    // ACCEPT Booking
    public async Task AcceptAsync(Guid bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);

        if (booking == null)
            throw new Exception("Booking not found");

        if (booking.State != BookingState.Pending)
            throw new Exception("Only pending bookings can be accepted");

        booking.State = BookingState.Accepted;
        await _context.SaveChangesAsync();
    }
    
    // COMPLETE Booking → Release escrow to provider
    public async Task CompleteAsync(Guid bookingId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var booking = await _context.Bookings
            .Include(b => b.Escrow)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
        
        if (booking == null)
            throw new Exception("Booking not found");

        if (booking.State != BookingState.Accepted)
            throw new Exception("Only accepted bookings can be completed");

        var clientWallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == booking.ClientId);
        
        if (clientWallet == null)
            throw new Exception("Client wallet not found");

        var providerWallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == booking.ProviderId);
        
        if (providerWallet == null)
            throw new Exception("Provider wallet not found");

        // Release escrow
        clientWallet.EscrowBalance -= booking.Escrow.Amount;
        providerWallet.AvailableBalance += booking.Escrow.Amount;

        booking.State = BookingState.Completed;
        booking.Escrow.Status = EscrowStatus.Released;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    
    // REJECT Booking → Refund escrow back to client
    public async Task RejectAsync(Guid bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Escrow)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
        
        if (booking == null)
            throw new Exception("Booking not found");

        if (booking.State != BookingState.Pending)
            throw new Exception("Only pending bookings can be rejected");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == booking.ClientId);
        
        if (wallet == null)
            throw new Exception("Client wallet not found");
        
        // Refund
        wallet.EscrowBalance -= booking.Escrow.Amount;
        wallet.AvailableBalance += booking.Escrow.Amount;

        booking.State = BookingState.Rejected;
        booking.Escrow.Status = EscrowStatus.Refunded;

        await _context.SaveChangesAsync();
    }
    public async Task<List<BookingResponseDto>> GetMyBookingsAsync(string userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.ClientId == userId || b.ProviderId == userId)
            .Include(b => b.Escrow)
            .ToListAsync();

        return bookings.Select(b => new BookingResponseDto
        {
            BookingId = b.Id,
            State = b.State,
            EscrowStatus = b.Escrow.Status
        }).ToList();
    }
    
    // old CREATE
    // public Booking Create(Guid clientId, CreateBookingDto dto)
    // {
    //     if (dto.EndTime <= dto.StartTime)
    //         throw new Exception("Invalid time slot");
    //
    //     var booking = new Booking
    //     {
    //         ClientId = clientId,
    //         ProviderId = dto.ProviderId,
    //         ListingId = dto.ListingId,
    //         StartTime = dto.StartTime,
    //         EndTime = dto.EndTime,
    //         Escrow = new EscrowTransaction
    //         {
    //             Amount = dto.Price,
    //             Status = EscrowStatus.Hold
    //         }
    //     };
    //
    //     return booking;
    // }

    
    // old ACCEPT ,COMPLETE ,AND REJECT
    // public void Accept(Booking booking)
    // {
    //     if (booking.State != BookingState.Pending)
    //         throw new Exception("Only pending bookings can be accepted");
    //
    //     booking.State = BookingState.Accepted;
    // }
    //
    // public void Complete(Booking booking)
    // {
    //     if (booking.State != BookingState.Accepted)
    //         throw new Exception("Only accepted bookings can be completed");
    //
    //     booking.State = BookingState.Completed;
    //     booking.Escrow.Status = EscrowStatus.Released;
    // }
    //
    // public void Reject(Booking booking)
    // {
    //     if (booking.State != BookingState.Pending)
    //         throw new Exception("Only pending bookings can be rejected");
    //
    //     booking.State = BookingState.Rejected;
    //     booking.Escrow.Status = EscrowStatus.Refunded;
    // }
}