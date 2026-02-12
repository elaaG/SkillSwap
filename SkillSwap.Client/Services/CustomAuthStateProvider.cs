using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace SkillSwap.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
{
    var token = await _localStorage.GetItemAsync<string>("authToken");

    if (string.IsNullOrEmpty(token))
    {
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }


    var claims = ParseClaimsFromJwt(token);
    var identity = new ClaimsIdentity(claims, "jwt");
    var user = new ClaimsPrincipal(identity);

    return new AuthenticationState(user);
}

        public async Task MarkUserAsAuthenticatedAsync(string token)
        {
            await _localStorage.SetItemAsync("authToken", token);

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                // Try multiple possible keys for the user ID claim
                if (keyValuePairs.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out var nameIdFull))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdFull.ToString()!));
                else if (keyValuePairs.TryGetValue("nameid", out var nameId))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId.ToString()!));
                else if (keyValuePairs.TryGetValue("sub", out var sub))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.ToString()!));

                if (keyValuePairs.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out var emailFull))
                    claims.Add(new Claim(ClaimTypes.Email, emailFull.ToString()!));
                else if (keyValuePairs.TryGetValue("email", out var email))
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString()!));

                if (keyValuePairs.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out var nameFull))
                    claims.Add(new Claim(ClaimTypes.Name, nameFull.ToString()!));
                else if (keyValuePairs.TryGetValue("name", out var name))
                    claims.Add(new Claim(ClaimTypes.Name, name.ToString()!));
                
                // Add all other claims as-is for debugging
                foreach (var kvp in keyValuePairs)
                {
                    if (!kvp.Key.Contains("nameidentifier") && !kvp.Key.Contains("email") && !kvp.Key.Contains("name"))
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString()!));
                    }
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
