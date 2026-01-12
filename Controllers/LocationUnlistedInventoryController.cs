using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationUnlistedInventoryController : ControllerBase
    {
        private readonly ILocationUnlistedInventoryService _service;

        public LocationUnlistedInventoryController(ILocationUnlistedInventoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all unlisted inventory items
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationUnlistedInventoryDto>>> GetAll()
        {
            try
            {
                var inventory = await _service.GetAllAsync();
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get unlisted inventory item by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationUnlistedInventoryDto>> GetById(int id)
        {
            try
            {
                var inventory = await _service.GetByIdAsync(id);
                if (inventory == null)
                    return NotFound($"Unlisted inventory with ID {id} not found");

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get unlisted inventory items by location ID
        /// </summary>
        [HttpGet("location/{locationId}")]
        public async Task<ActionResult<IEnumerable<LocationUnlistedInventoryDto>>> GetByLocationId(int locationId)
        {
            try
            {
                var inventory = await _service.GetByLocationIdAsync(locationId);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get unlisted inventory item by barcode and location
        /// </summary>
        [HttpGet("barcode/{barcodeNo}/location/{locationId}")]
        public async Task<ActionResult<LocationUnlistedInventoryDto>> GetByBarcodeAndLocation(string barcodeNo, int locationId)
        {
            try
            {
                var inventory = await _service.GetByBarcodeAndLocationAsync(barcodeNo, locationId);
                if (inventory == null)
                    return NotFound($"Unlisted inventory with barcode {barcodeNo} at location {locationId} not found");

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create new unlisted inventory item
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LocationUnlistedInventoryDto>> Create([FromBody] CreateLocationUnlistedInventoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var inventory = await _service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = inventory.Id }, inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update unlisted inventory item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<LocationUnlistedInventoryDto>> Update(int id, [FromBody] UpdateLocationUnlistedInventoryDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var inventory = await _service.UpdateAsync(id, updateDto);
                if (inventory == null)
                    return NotFound($"Unlisted inventory with ID {id} not found");

                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete unlisted inventory item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromQuery] int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("Valid user ID is required");

                var result = await _service.DeleteAsync(id, userId);
                if (!result)
                    return NotFound($"Unlisted inventory with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}