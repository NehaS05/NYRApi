using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransferInventoryController : ControllerBase
    {
        private readonly ITransferInventoryService _transferInventoryService;

        public TransferInventoryController(ITransferInventoryService transferInventoryService)
        {
            _transferInventoryService = transferInventoryService;
        }

        [HttpGet("locations")]
        [Authorize(Roles = "Admin,Customer,Staff")]
        public async Task<ActionResult<IEnumerable<TransferInventoryLocationDto>>> GetLocationsWithTransfers()
        {
            var locations = await _transferInventoryService.GetLocationsWithTransfersAsync();
            return Ok(locations);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Staff")]
        public async Task<ActionResult<IEnumerable<TransferInventoryDto>>> GetAllTransfers()
        {
            var transfers = await _transferInventoryService.GetAllTransfersAsync();
            return Ok(transfers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer,Staff")]
        public async Task<ActionResult<TransferInventoryDto>> GetTransfer(int id)
        {
            var transfer = await _transferInventoryService.GetTransferByIdAsync(id);
            if (transfer == null)
                return NotFound();

            return Ok(transfer);
        }

        [HttpGet("location/{locationId}/items")]
        [Authorize(Roles = "Admin,Customer,Staff")]
        public async Task<ActionResult<IEnumerable<TransferInventoryItemDto>>> GetTransferItemsByLocation(int locationId)
        {
            var items = await _transferInventoryService.GetTransferItemsByLocationIdAsync(locationId);
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<TransferInventoryDto>> CreateTransfer([FromBody] CreateTransferInventoryDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var transfer = await _transferInventoryService.CreateTransferAsync(createDto);
                return CreatedAtAction(nameof(GetTransfer), new { id = transfer.Id }, transfer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteTransfer(int id)
        {
            var result = await _transferInventoryService.DeleteTransferAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
