using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationOutwardInventoryController : ControllerBase
    {
        private readonly ILocationOutwardInventoryService _outwardInventoryService;
        private readonly ILocationUnlistedInventoryService _unlistedInventoryService;

        public LocationOutwardInventoryController(
            ILocationOutwardInventoryService outwardInventoryService,
            ILocationUnlistedInventoryService unlistedInventoryService)
        {
            _outwardInventoryService = outwardInventoryService;
            _unlistedInventoryService = unlistedInventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationOutwardInventoryDto>>> GetAllOutwardInventory()
        {
            var inventory = await _outwardInventoryService.GetAllOutwardInventoryAsync();
            return Ok(inventory);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocationOutwardInventoryDto>> GetOutwardInventory(int id)
        {
            var inventory = await _outwardInventoryService.GetOutwardInventoryByIdAsync(id);
            if (inventory == null)
                return NotFound();

            return Ok(inventory);
        }

        [HttpGet("by-location/{locationId}")]
        public async Task<ActionResult<IEnumerable<LocationOutwardInventoryDto>>> GetOutwardInventoryByLocation(int locationId, [FromQuery] bool? last60Minutes = null)
        {
            var inventory = await _outwardInventoryService.GetOutwardInventoryByLocationIdAsync(locationId, last60Minutes);
            return Ok(inventory);
        }

        [HttpGet("by-location/{locationId}/active")]
        public async Task<ActionResult<IEnumerable<LocationOutwardInventoryDto>>> GetActiveOutwardInventoryByLocation(int locationId)
        {
            var inventory = await _outwardInventoryService.GetActiveOutwardInventoryByLocationIdAsync(locationId);
            return Ok(inventory);
        }

        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IEnumerable<LocationOutwardInventoryDto>>> GetOutwardInventoryByProduct(int productId)
        {
            var inventory = await _outwardInventoryService.GetOutwardInventoryByProductIdAsync(productId);
            return Ok(inventory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Scanner")]
        public async Task<ActionResult<LocationOutwardInventoryDto>> CreateOutwardInventory([FromBody] CreateLocationOutwardInventoryDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventory = await _outwardInventoryService.CreateOutwardInventoryAsync(createDto);
                return CreatedAtAction(nameof(GetOutwardInventory), new { id = inventory.Id }, inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff,Scanner")]
        public async Task<ActionResult<LocationOutwardInventoryDto>> UpdateOutwardInventory(int id, [FromBody] UpdateLocationOutwardInventoryDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventory = await _outwardInventoryService.UpdateOutwardInventoryAsync(id, updateDto);
                if (inventory == null)
                    return NotFound();

                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin,Staff,Scanner")]
        public async Task<ActionResult> DeactivateOutwardInventory(int id, [FromBody] DeactivateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _outwardInventoryService.DeactivateOutwardInventoryAsync(id, request.UserId);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete outward inventory item (regular or unlisted)
        /// </summary>
        /// <param name="id">The ID of the inventory item to delete</param>
        /// <param name="request">Delete request containing UserId and ProductId</param>
        /// <returns>NoContent if successful, NotFound if item doesn't exist</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Scanner")]
        public async Task<ActionResult> DeleteOutwardInventory(int id, [FromBody] DeleteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (request.ProductId > 0)
                {
                    var result = await _outwardInventoryService.DeleteOutwardInventoryAsync(id, request.UserId);
                    if (!result)
                        return NotFound();
                }
                else if (request.ProductId == 0)
                {
                    // Delete unlisted inventory
                    var result = await _unlistedInventoryService.DeleteAsync(id, request.UserId);
                    if (!result)
                        return NotFound();
                }
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

    public class DeactivateRequest
    {
        public int UserId { get; set; }
    }

    public class DeleteRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}