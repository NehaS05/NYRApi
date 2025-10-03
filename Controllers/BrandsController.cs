using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetActiveBrands()
        {
            var brands = await _brandService.GetActiveBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDto>> GetBrand(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound();

            return Ok(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BrandDto>> CreateBrand([FromBody] CreateBrandDto createBrandDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var brand = await _brandService.CreateBrandAsync(createBrandDto);
                return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BrandDto>> UpdateBrand(int id, [FromBody] UpdateBrandDto updateBrandDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var brand = await _brandService.UpdateBrandAsync(id, updateBrandDto);
                if (brand == null)
                    return NotFound();

                return Ok(brand);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
