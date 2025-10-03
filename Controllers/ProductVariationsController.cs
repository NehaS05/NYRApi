using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductVariationsController : ControllerBase
    {
        private readonly IProductVariationService _variationService;

        public ProductVariationsController(IProductVariationService variationService)
        {
            _variationService = variationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVariationDto>>> GetAllVariations()
        {
            var variations = await _variationService.GetAllVariationsAsync();
            return Ok(variations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariationDto>> GetVariation(int id)
        {
            var variation = await _variationService.GetVariationByIdAsync(id);
            if (variation == null)
                return NotFound();

            return Ok(variation);
        }

        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductVariationDto>>> GetVariationsByProduct(int productId)
        {
            var variations = await _variationService.GetVariationsByProductAsync(productId);
            return Ok(variations);
        }

        [HttpGet("by-product/{productId}/type/{variationType}")]
        public async Task<ActionResult<IEnumerable<ProductVariationDto>>> GetVariationsByType(int productId, string variationType)
        {
            var variations = await _variationService.GetVariationsByTypeAsync(productId, variationType);
            return Ok(variations);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductVariationDto>> CreateVariation([FromBody] CreateProductVariationDto createVariationDto)
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
        public async Task<ActionResult<ProductVariationDto>> UpdateVariation(int id, [FromBody] UpdateProductVariationDto updateVariationDto)
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
