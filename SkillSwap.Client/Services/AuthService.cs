using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SkillSwap.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SkillSwap.Client.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient httpClient, 
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (authResponse != null)
                    {
                        await SaveTokenAsync(authResponse.Token);
                        await _localStorage.SetItemAsync("user", authResponse);

                        // Notify auth state changed
                        if (_authStateProvider is CustomAuthStateProvider customProvider)
                        {
                            await customProvider.MarkUserAsAuthenticatedAsync(authResponse.Token);
                        }

                        return new ApiResponse<AuthResponse> { Success = true, Data = authResponse };
                    }
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    ErrorMessage = error?.Message ?? "Registration failed"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (authResponse != null)
                    {
                        await SaveTokenAsync(authResponse.Token);
                        await _localStorage.SetItemAsync("user", authResponse);

                        // Notify auth state changed
                        if (_authStateProvider is CustomAuthStateProvider customProvider)
                        {
                            await customProvider.MarkUserAsAuthenticatedAsync(authResponse.Token);
                        }

                        return new ApiResponse<AuthResponse> { Success = true, Data = authResponse };
                    }
                }

                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    ErrorMessage = error?.Message ?? "Login failed"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("user");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Notify auth state changed
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                await customProvider.MarkUserAsLoggedOutAsync();
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }

        private async Task SaveTokenAsync(string token)
{
            await _localStorage.SetItemAsync("authToken", token);
}
    }
}