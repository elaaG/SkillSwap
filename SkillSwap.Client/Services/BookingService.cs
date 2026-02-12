using SkillSwap.Client.Models;
using System.Net.Http.Json;

namespace SkillSwap.Client.Services
{
    public interface IBookingService
    {
        Task<ApiResponse<List<BookingDto>>> GetMyBookingsAsync();
        Task<ApiResponse<bool>> AcceptBookingAsync(Guid bookingId);
        Task<ApiResponse<bool>> CompleteBookingAsync(Guid bookingId);
        Task<ApiResponse<bool>> RejectBookingAsync(Guid bookingId);
		Task<ApiResponse<BookingDto>> CreateBookingAsync(CreateBookingDto dto);

    }

    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<List<BookingDto>>> GetMyBookingsAsync()
        {
            try
            {
                var bookings = await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
                
                if (bookings != null)
                {
                    return new ApiResponse<List<BookingDto>> 
                    { 
                        Success = true, 
                        Data = bookings 
                    };
                }

                return new ApiResponse<List<BookingDto>>
                {
                    Success = false,
                    ErrorMessage = "Failed to load bookings"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BookingDto>>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<bool>> AcceptBookingAsync(Guid bookingId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/bookings/{bookingId}/accept", null);
                
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool> { Success = true, Data = true };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = error
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<bool>> CompleteBookingAsync(Guid bookingId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/bookings/{bookingId}/complete", null);
                
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool> { Success = true, Data = true };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = error
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<bool>> RejectBookingAsync(Guid bookingId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/bookings/{bookingId}/reject", null);
                
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool> { Success = true, Data = true };
                }

                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = error
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        
public async Task<ApiResponse<BookingDto>> CreateBookingAsync(CreateBookingDto dto)
{
    try
    {
        var response = await _httpClient.PostAsJsonAsync("api/bookings", dto);

        if (response.IsSuccessStatusCode)
        {
            var booking = await response.Content.ReadFromJsonAsync<BookingDto>();

            return new ApiResponse<BookingDto>
            {
                Success = true,
                Data = booking
            };
        }

        var error = await response.Content.ReadAsStringAsync();
        return new ApiResponse<BookingDto>
        {
            Success = false,
            ErrorMessage = error
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<BookingDto>
        {
            Success = false,
            ErrorMessage = ex.Message
        };
    }
}


    }
}