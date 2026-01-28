using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VanInventoryController : ControllerBase
    {
        private readonly IVanInventoryService _vanInventoryService;

        public VanInventoryController(IVanInventoryService vanInventoryService)
        {
            _vanInventoryService = vanInventoryService;
        }

        [HttpGet("vans")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult> GetVansWithTransfers([FromQuery] PaginationParamsDto? paginationParams = null)
        {
            // Use pagination if any query parameters are provided, otherwise return all (backward compatibility)
            if (paginationParams != null && (Request.Query.ContainsKey("pageNumber") || Request.Query.ContainsKey("pageSize")))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _vanInventoryService.GetVansWithTransfersPagedAsync(paginationParams);
                return Ok(result);
            }

            var vans = await _vanInventoryService.GetVansWithTransfersAsync();
            return Ok(vans);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<VanInventoryDto>>> GetAllTransfers()
        {
            var transfers = await _vanInventoryService.GetAllTransfersAsync();
            return Ok(transfers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<VanInventoryDto>> GetTransfer(int id)
        {
            var transfer = await _vanInventoryService.GetTransferByIdAsync(id);
            if (transfer == null)
                return NotFound();

            return Ok(transfer);
        }

        [HttpGet("van/{vanId}/items")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<VanInventoryItemDto>>> GetTransferItemsByVan(int vanId)
        {
            var items = await _vanInventoryService.GetTransferItemsByVanIdAsync(vanId);
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<VanInventoryDto>> CreateTransfer([FromBody] CreateVanInventoryDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var transfer = await _vanInventoryService.CreateTransferAsync(createDto);
                return CreatedAtAction(nameof(GetTransfer), new { id = transfer.Id }, transfer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<ActionResult> DeleteTransfer(int id)
        {
            var result = await _vanInventoryService.DeleteTransferAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("tracking")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<TransferTrackingDto>>> GetAllTransfersTracking()
        {
            var transfers = await _vanInventoryService.GetAllTransfersTrackingAsync();
            return Ok(transfers);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<TransferTrackingDto>> UpdateTransferStatus(int id, [FromBody] UpdateTransferStatusDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transfer = await _vanInventoryService.UpdateTransferStatusAsync(id, updateDto);
            if (transfer == null)
                return NotFound();

            return Ok(transfer);
        }
    }
}
