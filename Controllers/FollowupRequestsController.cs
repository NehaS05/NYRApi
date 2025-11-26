using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FollowupRequestsController : ControllerBase
    {
        private readonly IFollowupRequestService _followupRequestService;

        public FollowupRequestsController(IFollowupRequestService followupRequestService)
        {
            _followupRequestService = followupRequestService;
        }

        /// <summary>
        /// Get all followup requests
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<FollowupRequestDto>>> GetAllFollowupRequests()
        {
            var followupRequests = await _followupRequestService.GetAllFollowupRequestsAsync();
            return Ok(followupRequests);
        }

        /// <summary>
        /// Get followup request by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<FollowupRequestDto>> GetFollowupRequest(int id)
        {
            var followupRequest = await _followupRequestService.GetFollowupRequestByIdAsync(id);
            if (followupRequest == null)
                return NotFound();

            return Ok(followupRequest);
        }

        /// <summary>
        /// /// Get followup requests by location ID
        /// </summary>
        /// [HttpGet("location/{locationId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<FollowupRequestDto>>> GetFollowupRequestsByLocation(int locationId)
        {
            var followupRequests = await _followupRequestService.GetFollowupRequestsByLocationIdAsync(locationId);
            return Ok(followupRequests);
        }

        /// <summary>
        /// /// Get followup requests by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<FollowupRequestDto>>> GetFollowupRequestsByCustomer(int customerId)
        {
            var followupRequests = await _followupRequestService.GetFollowupRequestsByCustomerIdAsync(customerId);
            return Ok(followupRequests);
        }

        /// <summary>
        /// Get followup requests by status
        /// /// </summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<FollowupRequestDto>>> GetFollowupRequestsByStatus(string status)
        {
            var followupRequests = await _followupRequestService.GetFollowupRequestsByStatusAsync(status);
            return Ok(followupRequests);
        }

        /// <summary>
        /// Create a new followup request
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<FollowupRequestDto>> CreateFollowupRequest([FromBody] CreateFollowupRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var followupRequest = await _followupRequestService.CreateFollowupRequestAsync(createDto);
            return CreatedAtAction(nameof(GetFollowupRequest), new { id = followupRequest.Id }, followupRequest);
        }

        /// <summary>
        /// Update followup request status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<FollowupRequestDto>> UpdateFollowupRequestStatus(int id, [FromBody] UpdateFollowupRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var followupRequest = await _followupRequestService.UpdateFollowupRequestStatusAsync(id, updateDto);
            if (followupRequest == null)
                return NotFound();

            return Ok(followupRequest);
        }

        /// <summary>
        /// Delete (soft delete) a followup request
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteFollowupRequest(int id)
        {
            var result = await _followupRequestService.DeleteFollowupRequestAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
