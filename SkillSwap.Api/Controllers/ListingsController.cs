using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.API.DTOs.Listing;
using SkillSwap.API.Services.Interfaces;

namespace SkillSwap.API.Controllers
{
    [ApiController]
    [Route("api/listings")]
    public class ListingsController : ControllerBase
    {
        private readonly IListingService _service;

        public ListingsController(IListingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? skillId,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(search, skillId, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var listing = await _service.GetByIdAsync(id);
            if (listing == null) return NotFound();
            return Ok(listing);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ListingCreateDto dto)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(providerId))
                return Unauthorized();
            try
            {
                var created = await _service.CreateAsync(providerId, dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ListingUpdateDto dto)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(providerId))
                return Unauthorized();

           try
            {
                var updated = await _service.UpdateAsync(id, providerId, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(providerId))
                return Unauthorized();

            try
            {
                var ok = await _service.DeleteAsync(id, providerId);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
