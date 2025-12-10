using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScannersController : ControllerBase
    {
        private readonly IScannerService _scannerService;

        public ScannersController(IScannerService scannerService)
        {
            _scannerService = scannerService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<ScannerDto>>> GetAllScanners()
        {
            var scanners = await _scannerService.GetAllScannersAsync();
            return Ok(scanners);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ScannerDto>> GetScanner(int id)
        {
            var scanner = await _scannerService.GetScannerByIdAsync(id);
            if (scanner == null)
                return NotFound();

            return Ok(scanner);
        }

        [HttpGet("by-location/{locationId}")]
        [Authorize(Roles = "Admin,Staff,Scanner")] // Allow scanners to see scanners in their location
        public async Task<ActionResult<IEnumerable<ScannerDto>>> GetScannersByLocation(int locationId)
        {
            // If it's a scanner token, ensure they can only access their own location
            if (User.IsInRole("Scanner"))
            {
                var tokenLocationId = User.FindFirst("LocationId")?.Value;
                if (tokenLocationId != locationId.ToString())
                {
                    return Forbid("Scanners can only access their own location");
                }
            }

            var scanners = await _scannerService.GetScannersByLocationAsync(locationId);
            return Ok(scanners);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<ScannerDto>>> SearchScanners([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var scanners = await _scannerService.SearchScannersAsync(searchTerm);
            return Ok(scanners);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ScannerDto>> CreateScanner([FromBody] CreateScannerDto createScannerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var scanner = await _scannerService.CreateScannerAsync(createScannerDto);
                return CreatedAtAction(nameof(GetScanner), new { id = scanner.Id }, scanner);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ScannerDto>> UpdateScanner(int id, [FromBody] UpdateScannerDto updateScannerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var scanner = await _scannerService.UpdateScannerAsync(id, updateScannerDto);
                if (scanner == null)
                    return NotFound();

                return Ok(scanner);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteScanner(int id)
        {
            var result = await _scannerService.DeleteScannerAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("confirm-pin")]
        [AllowAnonymous] // Allow anonymous access for scanner PIN confirmation
        public async Task<ActionResult<ScannerPinConfirmResponseDto>> ConfirmScannerPin([FromBody] ScannerPinConfirmDto confirmDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _scannerService.ConfirmScannerPinAsync(confirmDto);
                
                if (!result.IsValid)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ScannerPinConfirmResponseDto
                {
                    IsValid = false,
                    Message = "An error occurred while confirming PIN",
                    Scanner = null
                });
            }
        }

        [HttpPost("reset-pin")]
        [Authorize(Roles = "Admin,Staff,Scanner")] // Require authentication for PIN reset
        public async Task<ActionResult<ScannerPinResetResponseDto>> ResetScannerPin([FromBody] ScannerPinResetDto resetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _scannerService.ResetScannerPinAsync(resetDto);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ScannerPinResetResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while resetting PIN"
                });
            }
        }

        [HttpGet("test-token")]
        [Authorize(Roles = "Scanner")] // Test endpoint for scanner tokens
        public async Task<ActionResult> TestScannerToken()
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            
            return Ok(new
            {
                Message = "Scanner token is valid!",
                Claims = userClaims,
                ScannerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                ScannerName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
                SerialNo = User.FindFirst("SerialNo")?.Value,
                LocationId = User.FindFirst("LocationId")?.Value,
                Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
            });
        }

        [HttpGet("my-info")]
        [Authorize(Roles = "Scanner")] // Allow scanners to get their own info
        public async Task<ActionResult<ScannerDto>> GetMyInfo()
        {
            var scannerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (!int.TryParse(scannerIdClaim, out int scannerId))
            {
                return BadRequest("Invalid scanner token");
            }

            var scanner = await _scannerService.GetScannerByIdAsync(scannerId);
            if (scanner == null)
                return NotFound("Scanner not found");

            return Ok(scanner);
        }
    }
}

