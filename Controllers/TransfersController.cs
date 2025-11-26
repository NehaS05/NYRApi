using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        /// <summary>
        /// Get all transfers (van transfers and restock requests combined)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<TransferDto>>> GetAllTransfers()
        {
            var transfers = await _transferService.GetAllTransfersAsync();
            return Ok(transfers);
        }

        /// <summary>
        /// Get transfer by ID and type
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<TransferDto>> GetTransfer(int id, [FromQuery] string type)
        {
            if (string.IsNullOrEmpty(type))
                return BadRequest("Type parameter is required (VanTransfer or RestockRequest)");

            var transfer = await _transferService.GetTransferByIdAsync(id, type);
            if (transfer == null)
                return NotFound();

            return Ok(transfer);
        }

        /// <summary>
        /// Get transfers by location ID
        /// </summary>
        [HttpGet("location/{locationId}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<TransferDto>>> GetTransfersByLocation(int locationId)
        {
            var transfers = await _transferService.GetTransfersByLocationIdAsync(locationId);
            return Ok(transfers);
        }

        /// <summary>
        /// Get transfers by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<TransferDto>>> GetTransfersByCustomer(int customerId)
        {
            var transfers = await _transferService.GetTransfersByCustomerIdAsync(customerId);
            return Ok(transfers);
        }

        /// <summary>
        /// Get transfers by status
        /// </summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<TransferDto>>> GetTransfersByStatus(string status)
        {
            var transfers = await _transferService.GetTransfersByStatusAsync(status);
            return Ok(transfers);
        }

        /// <summary>
        /// Get transfers by type (VanTransfer or RestockRequest)
        /// </summary>
        [HttpGet("type/{type}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<TransferDto>>> GetTransfersByType(string type)
        {
            if (!type.Equals("VanTransfer", StringComparison.OrdinalIgnoreCase) && 
                !type.Equals("RestockRequest", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Type must be either 'VanTransfer' or 'RestockRequest'");
            }

            var transfers = await _transferService.GetTransfersByTypeAsync(type);
            return Ok(transfers);
        }

        /// <summary>
        /// Get transfers summary statistics
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<TransferSummaryDto>> GetTransfersSummary()
        {
            var summary = await _transferService.GetTransfersSummaryAsync();
            return Ok(summary);
        }
    }
}
