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
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<ScannerDto>>> GetScannersByLocation(int locationId)
        {
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
    }
}

