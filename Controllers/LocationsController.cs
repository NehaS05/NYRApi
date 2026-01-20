using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        [HttpGet("locationDetails")]
        [Authorize(Roles = "Admin,Customer,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAllLocationsWithInventory()
        {
            try
            {
                var locations = await _locationService.GetAllLocationsWithInventoryAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer,Staff,Driver")]
        public async Task<ActionResult<LocationDto>> GetLocation(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
                return NotFound();

            return Ok(location);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationDto createLocationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var location = await _locationService.CreateLocationAsync(createLocationDto);
                return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(int id, [FromBody] UpdateLocationDto updateLocationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var location = await _locationService.UpdateLocationAsync(id, updateLocationDto);
                if (location == null)
                    return NotFound();

                return Ok(location);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            var result = await _locationService.DeleteLocationAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("by-customer/{customerId}")]
        [Authorize(Roles = "Admin,Customer,Staff")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocationsByCustomer(int customerId)
        {
            var locations = await _locationService.GetLocationsByCustomerAsync(customerId);
            return Ok(locations);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Customer,Staff")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> SearchLocations([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var locations = await _locationService.SearchLocationsAsync(searchTerm);
            return Ok(locations);
        }

        [HttpGet("follow-up-needed")]
        [Authorize(Roles = "Admin,Staff,Scanner")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocationsNeedingFollowUp()
        {
            try
            {
                var locations = await _locationService.GetLocationsNeedingFollowUpAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Get locations that don't have any scanners assigned
        /// </summary>
        [HttpGet("Location-without-scanners")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocationsWithoutScanners()
        {
            try
            {
                var locations = await _locationService.GetLocationsWithoutScannersAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
