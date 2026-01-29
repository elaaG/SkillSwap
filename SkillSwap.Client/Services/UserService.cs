using Blazored.LocalStorage;
using SkillSwap.Client.Models;
using System.Net.Http.Json;

namespace SkillSwap.Client.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserProfile>> GetProfileAsync();
        Task<ApiResponse<UserProfile>> UpdateProfileAsync(UpdateProfileRequest request);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<UserProfile>> GetProfileAsync()
        {
            try
            {
                var profile = await _httpClient.GetFromJsonAsync<UserProfile>("api/user/profile");

                if (profile != null)
                {
                    return new ApiResponse<UserProfile> { Success = true, Data = profile };
                }

                return new ApiResponse<UserProfile>
                {
                    Success = false,
                    ErrorMessage = "Failed to load profile"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserProfile>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<UserProfile>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/user/profile", request);

                if (response.IsSuccessStatusCode)
                {
                    var profile = await response.Content.ReadFromJsonAsync<UserProfile>();
                    if (profile != null)
                    {
                        return new ApiResponse<UserProfile> { Success = true, Data = profile };
                    }
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return new ApiResponse<UserProfile>
                {
                    Success = false,
                    ErrorMessage = error?.Message ?? "Failed to update profile"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserProfile>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}