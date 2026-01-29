using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WarehouseInventoryController : ControllerBase
    {
        private readonly IWarehouseInventoryService _warehouseInventoryService;

        public WarehouseInventoryController(IWarehouseInventoryService warehouseInventoryService)
        {
            _warehouseInventoryService = warehouseInventoryService;
        }

        /// <summary>
        /// Add inventory to a warehouse
        /// </summary>
        /// <param name="addInventoryDto">Inventory details to add</param>
        /// <returns>Created inventory item</returns>
        [HttpPost("add-inventory")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<WarehouseInventoryDto>> AddInventory([FromBody] AddInventoryDto addInventoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventory = await _warehouseInventoryService.AddInventoryAsync(addInventoryDto);
                return CreatedAtAction(nameof(GetInventoryById), new { id = inventory.Id }, inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add multiple inventory items to a warehouse for a single product
        /// </summary>
        /// /// <param name="addBulkInventoryDto">Bulk inventory details to add</param>
        /// <returns>List of created inventory items</returns>
        [HttpPost("add-bulk-inventory")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<WarehouseInventoryDto>>> AddBulkInventory([FromBody] AddBulkInventoryDto addBulkInventoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventoryItems = await _warehouseInventoryService.AddBulkInventoryAsync(addBulkInventoryDto);
                return Ok(inventoryItems);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get list of warehouses with inventory summary
        /// </summary>
        /// <returns>List of warehouses with product counts and total quantities</returns>
        [HttpGet("warehouses")]
        public async Task<ActionResult> GetWarehouseList([FromQuery] PaginationParamsDto? paginationParams = null)
        {
            // Use pagination if any query parameters are provided, otherwise return all (backward compatibility)
            if (paginationParams != null && (Request.Query.ContainsKey("pageNumber") || Request.Query.ContainsKey("pageSize")))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _warehouseInventoryService.GetWarehouseListPagedAsync(paginationParams);
                return Ok(result);
            }

            var warehouses = await _warehouseInventoryService.GetWarehouseListAsync();
            return Ok(warehouses);
        }

        /// <summary>
        /// Get detailed inventory for a specific warehouse
        /// </summary>
        /// <param name="warehouseId">Warehouse ID</param>
        /// <param name="paginationParams">Optional pagination parameters</param>
        /// <returns>List of inventory items in the warehouse</returns>
        [HttpGet("warehouse/{warehouseId}/inventory")]
        public async Task<ActionResult> GetWarehouseInventoryDetails(int warehouseId, [FromQuery] PaginationParamsDto? paginationParams = null)
        {
            if (paginationParams != null && (Request.Query.ContainsKey("pageNumber") || Request.Query.ContainsKey("pageSize")))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _warehouseInventoryService.GetWarehouseInventoryDetailsPagedAsync(warehouseId, paginationParams);
                return Ok(result);
            }

            var inventory = await _warehouseInventoryService.GetWarehouseInventoryDetailsAsync(warehouseId);
            return Ok(inventory);
        }

        /// <summary>
        /// Get specific inventory item by ID
        /// </summary>
        /// <param name="id">Inventory item ID</param>
        /// <returns>Inventory item details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseInventoryDto>> GetInventoryById(int id)
        {
            var inventory = await _warehouseInventoryService.GetInventoryByIdAsync(id);
            if (inventory == null)
                return NotFound();

            return Ok(inventory);
        }

        /// <summary>
        /// Update inventory item
        /// </summary>
        /// <param name="id">Inventory item ID</param>
        /// <param name="updateInventoryDto">Updated inventory details</param>
        /// <returns>Updated inventory item</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<WarehouseInventoryDto>> UpdateInventory(int id, [FromBody] UpdateInventoryDto updateInventoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = await _warehouseInventoryService.UpdateInventoryAsync(id, updateInventoryDto);
            if (inventory == null)
                return NotFound();

            return Ok(inventory);
        }

        /// <summary>
        /// Delete inventory item (soft delete)
        /// </summary>
        /// <param name="id">Inventory item ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> DeleteInventory(int id)
        {
            var result = await _warehouseInventoryService.DeleteInventoryAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Get all inventory items for a specific warehouse
        /// </summary>
        /// <param name="warehouseId">Warehouse ID</param>
        /// <returns>List of inventory items</returns>
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<WarehouseInventoryDto>>> GetInventoryByWarehouse(int warehouseId)
        {
            var inventory = await _warehouseInventoryService.GetInventoryByWarehouseAsync(warehouseId);
            return Ok(inventory);
        }

        /// <summary>
        /// Get all inventory items for a specific product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>List of inventory items</returns>
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<WarehouseInventoryDto>>> GetInventoryByProduct(int productId)
        {
            var inventory = await _warehouseInventoryService.GetInventoryByProductAsync(productId);
            return Ok(inventory);
        }

        /// <summary>
        /// Check if inventory exists for specific warehouse and product variation
        /// </summary>
        /// <param name="warehouseId">Warehouse ID</param>
        /// <param name="productVariationId">Product variation ID</param>
        /// <returns>Existence status</returns>
        [HttpGet("exists")]
        public async Task<ActionResult<bool>> CheckInventoryExists(int warehouseId, int productVariationId)
        {
            var exists = await _warehouseInventoryService.ExistsByWarehouseAndProductVariantAsync(warehouseId, productVariationId);
            return Ok(exists);
        }
    }
}
