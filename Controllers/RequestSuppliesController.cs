using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RequestSuppliesController : ControllerBase
    {
        private readonly IRequestSupplyService _requestSupplyService;

        public RequestSuppliesController(IRequestSupplyService requestSupplyService)
        {
            _requestSupplyService = requestSupplyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestSupplyDto>>> GetAllRequestSupplies()
        {
            var requestSupplies = await _requestSupplyService.GetAllRequestSuppliesAsync();
            return Ok(requestSupplies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestSupplyDto>> GetRequestSupply(int id)
        {
            var requestSupply = await _requestSupplyService.GetRequestSupplyByIdAsync(id);
            if (requestSupply == null)
                return NotFound();

            return Ok(requestSupply);
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<RequestSupplyDto>>> GetRequestSuppliesByStatus(string status)
        {
            var requestSupplies = await _requestSupplyService.GetRequestSuppliesByStatusAsync(status);
            return Ok(requestSupplies);
        }

        [HttpPost]
        public async Task<ActionResult<RequestSupplyDto>> CreateRequestSupply([FromBody] CreateRequestSupplyDto createRequestSupplyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var requestSupply = await _requestSupplyService.CreateRequestSupplyAsync(createRequestSupplyDto);
                return CreatedAtAction(nameof(GetRequestSupply), new { id = requestSupply.Id }, requestSupply);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RequestSupplyDto>> UpdateRequestSupply(int id, [FromBody] UpdateRequestSupplyDto updateRequestSupplyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var requestSupply = await _requestSupplyService.UpdateRequestSupplyAsync(id, updateRequestSupplyDto);
                if (requestSupply == null)
                    return NotFound();

                return Ok(requestSupply);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRequestSupply(int id)
        {
            var result = await _requestSupplyService.DeleteRequestSupplyAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
