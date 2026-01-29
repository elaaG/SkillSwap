using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkillSwap.API.Data;
using SkillSwap.API.DTOs.Auth;
using SkillSwap.API.Exceptions;
using SkillSwap.API.Models;
using SkillSwap.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillSwap.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        
        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // awel haja Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("Email is already registered");
            }
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // sinon Create user
                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Registration failed: {errors}");
                }
                
                // Create wallet with initial credits
                var wallet = new Wallet
                {
                    UserId = user.Id,
                    AvailableBalance = 2.0m, 
                    EscrowBalance = 0m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.Wallets.Add(wallet);
                
                // Log the initial credit transaction
                var transactionLog = new TransactionLog
                {
                    ToUserId = user.Id,
                    Amount = 2.0m,
                    Type = TransactionType.InitialCredit,
                    Timestamp = DateTime.UtcNow,
                    Notes = "Welcome bonus - 2 free time credits",
                    TransactionReference = Guid.NewGuid().ToString()
                };
                
                _context.TransactionLogs.Add(transactionLog);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation("User {Email} registered successfully with wallet", registerDto.Email);
                
                // Generate JWT token
                var token = await GenerateJwtToken(user.Id, user.Email!, user.FullName);
                
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                    AvailableBalance = wallet.AvailableBalance,
                    EscrowBalance = wallet.EscrowBalance
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }
            
            if (!user.IsActive)
            {
                throw new UnauthorizedException("Account is deactivated");
            }
            
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            
            if (!isPasswordValid)
            {
                throw new UnauthorizedException("Invalid email or password");
            }
            
            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            
            // Get wallet info
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == user.Id);
            
            if (wallet == null)
            {
                throw new NotFoundException("Wallet not found for user");
            }
            
            var token = await GenerateJwtToken(user.Id, user.Email!, user.FullName);
            
            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
            
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                AvailableBalance = wallet.AvailableBalance,
                EscrowBalance = wallet.EscrowBalance
            };
        }
        
        public async Task<string> GenerateJwtToken(string userId, string email, string fullName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings["ExpireMinutes"])),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}