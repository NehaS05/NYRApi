using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationInventoryDataController : ControllerBase
    {
        private readonly ILocationInventoryDataService _inventoryService;

        public LocationInventoryDataController(ILocationInventoryDataService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationInventoryDataDto>>> GetAllInventory()
        {
            var inventory = await _inventoryService.GetAllInventoryAsync();
            return Ok(inventory);
        }

        [HttpGet("grouped-by-location")]
        public async Task<ActionResult> GetAllInventoryGroupedByLocation([FromQuery] PaginationParamsDto? paginationParams = null)
        {
            try
            {
                // Use pagination if any query parameters are provided, otherwise return all (backward compatibility)
                if (paginationParams != null && (Request.Query.ContainsKey("pageNumber") || Request.Query.ContainsKey("pageSize")))
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);

                    var pagedResult = await _inventoryService.GetAllInventoryGroupedByLocationPagedAsync(paginationParams);
                    return Ok(pagedResult);
                }

                var groupedInventory = await _inventoryService.GetAllInventoryGroupedByLocationAsync();
                return Ok(groupedInventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocationInventoryDataDto>> GetInventory(int id)
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(id);
            if (inventory == null)
                return NotFound();

            return Ok(inventory);
        }

        [HttpGet("by-location/{locationId}")]
        public async Task<ActionResult<IEnumerable<LocationInventoryDataDto>>> GetInventoryByLocation(int locationId)
        {
            var inventory = await _inventoryService.GetInventoryByLocationIdAsync(locationId);
            return Ok(inventory);
        }

        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IEnumerable<LocationInventoryDataDto>>> GetInventoryByProduct(int productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            return Ok(inventory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<LocationInventoryDataDto>> CreateInventory([FromBody] CreateLocationInventoryDataDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventory = await _inventoryService.CreateInventoryAsync(createDto);
                return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<LocationInventoryDataDto>> UpdateInventory(int id, [FromBody] UpdateLocationInventoryDataDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventory = await _inventoryService.UpdateInventoryAsync(id, updateDto);
                if (inventory == null)
                    return NotFound();

                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/adjust-quantity")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<LocationInventoryDataDto>> AdjustQuantity(int id, [FromBody] AdjustQuantityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventory = await _inventoryService.AdjustQuantityAsync(id, request.QuantityChange, request.UserId);
                if (inventory == null)
                    return NotFound();

                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteInventory(int id)
        {
            var result = await _inventoryService.DeleteInventoryAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("barcodeLocationScan/{barcode}")]
        [Authorize(Roles = "Admin,Staff,Scanner,Driver")]
        public async Task<ActionResult<IEnumerable<ProductVariantInfoDto>>> GetVariantInfoBySku(string barcode, [FromQuery] int locationId, [FromQuery] int? userId = null, [FromQuery] int? productVariantId = null)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return BadRequest("Barcode cannot be empty");

            var variantInfo = await _inventoryService.GetVariantInfoBySkuAsync(barcode, locationId, userId, productVariantId);
            //if (!variantInfo.Any())
            //    return Ok($"No product variants found in inventory with Barcode: {barcode}");

            return Ok(variantInfo);
        }
    }

    public class AdjustQuantityRequest
    {
        public int QuantityChange { get; set; }
        public int UserId { get; set; }
    }
}
