using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RestockRequestController : ControllerBase
    {
        private readonly IRestockRequestService _restockRequestService;

        public RestockRequestController(IRestockRequestService restockRequestService)
        {
            _restockRequestService = restockRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<RestockRequestDto>>> GetAllRequests()
        {
            var requests = await _restockRequestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<RestockRequestDto>> GetRequest(int id)
        {
            var request = await _restockRequestService.GetRequestByIdAsync(id);
            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpGet("location/{locationId}")]
        [Authorize(Roles = "Admin,Staff,Scanner")]
        public async Task<ActionResult<IEnumerable<RestockRequestDto>>> GetRequestsByLocation(int locationId)
        {
            var requests = await _restockRequestService.GetRequestsByLocationIdAsync(locationId);
            return Ok(requests);
        }

        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<RestockRequestDto>>> GetRequestsByCustomer(int customerId)
        {
            var requests = await _restockRequestService.GetRequestsByCustomerIdAsync(customerId);
            return Ok(requests);
        }

        [HttpGet("summary")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<RestockRequestSummaryDto>>> GetRequestsSummary()
        {
            var summary = await _restockRequestService.GetRequestsSummaryAsync();
            return Ok(summary);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<RestockRequestDto>> CreateRequest([FromBody] CreateRestockRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var request = await _restockRequestService.CreateRequestAsync(createDto);
                return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, request);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRequest(int id)
        {
            var result = await _restockRequestService.DeleteRequestAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
