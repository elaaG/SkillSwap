using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.API.DTOs.Wallet;
using SkillSwap.API.Helpers;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;
        
        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }
        
        
        [HttpGet]
        [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WalletDto>> GetWallet()
        {
            var userId = User.GetUserId();
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            
            return Ok(wallet);
        }
        
       
        [HttpGet("balance")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetBalance()
        {
            var userId = User.GetUserId();
            var balance = await _walletService.GetAvailableBalanceAsync(userId);
            
            return Ok(new { availableBalance = balance });
        }
        
        
        
        [HttpGet("transactions")]
        [ProducesResponseType(typeof(TransactionHistoryResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionHistoryResponseDto>> GetTransactionHistory(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = User.GetUserId();
            var history = await _walletService.GetTransactionHistoryAsync(userId, page, pageSize);
            
            return Ok(history);
        }
    }
}