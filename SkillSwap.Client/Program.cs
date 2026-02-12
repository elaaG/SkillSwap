using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SkillSwap.Client;
using SkillSwap.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
{
    var authHandler = sp.GetRequiredService<AuthHeaderHandler>();
    authHandler.InnerHandler = new HttpClientHandler();
    
    var httpClient = new HttpClient(authHandler)
    {
        BaseAddress = new Uri("http://localhost:5001")
    };
    
    return httpClient;
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IListingService, ListingService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<LocalSkillStore>();   // local skill storage (teaches/needs)

await builder.Build().RunAsync();