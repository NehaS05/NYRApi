using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VariationsController : ControllerBase
    {
        private readonly IVariationService _variationService;

        public VariationsController(IVariationService variationService)
        {
            _variationService = variationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VariationDto>>> GetAllVariations()
        {
            var variations = await _variationService.GetAllVariationsAsync();
            return Ok(variations);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<VariationDto>>> GetActiveVariations()
        {
            var variations = await _variationService.GetActiveVariationsAsync();
            return Ok(variations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VariationDto>> GetVariation(int id)
        {
            var variation = await _variationService.GetVariationByIdAsync(id);
            if (variation == null)
                return NotFound();

            return Ok(variation);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VariationDto>> CreateVariation([FromBody] CreateVariationDto createVariationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var variation = await _variationService.CreateVariationAsync(createVariationDto);
                return CreatedAtAction(nameof(GetVariation), new { id = variation.Id }, variation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VariationDto>> UpdateVariation(int id, [FromBody] UpdateVariationDto updateVariationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var variation = await _variationService.UpdateVariationAsync(id, updateVariationDto);
                if (variation == null)
                    return NotFound();

                return Ok(variation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteVariation(int id)
        {
            var result = await _variationService.DeleteVariationAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
