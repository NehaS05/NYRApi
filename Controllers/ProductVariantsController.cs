using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductVariantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetProductVariants(int productId)
        {
            var variants = await _context.ProductVariants
                .Include(p => p.Product)
                .Include(v => v.Attributes)
                    .ThenInclude(a => a.Variation)
                .Include(v => v.Attributes)
                    .ThenInclude(a => a.VariationOption)
                .Where(v => v.ProductId == productId && v.IsActive)
                .ToListAsync();

            var result = variants.Select(v => new ProductVariantDto
            {
                Id = v.Id,
                ProductId = v.ProductId,
                VariantName = v.VariantName,
                SKU = (v.SKU == ""  ? (v.BarcodeSKU) : v.SKU),
                Price = v.Price,
                IsEnabled = v.IsEnabled,
                Attributes = v.Attributes.Select(a => new ProductVariantAttributeDto
                {
                    Id = a.Id,
                    VariationId = a.VariationId,
                    VariationName = a.Variation.Name,
                    VariationOptionId = a.VariationOptionId,
                    VariationOptionName = a.VariationOption.Name,
                    VariationOptionValue = a.VariationOption.Value
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariantDto>> GetVariant(int id)
        {
            var variant = await _context.ProductVariants
                .Include(v => v.Attributes)
                    .ThenInclude(a => a.Variation)
                .Include(v => v.Attributes)
                    .ThenInclude(a => a.VariationOption)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (variant == null)
                return NotFound();

            var result = new ProductVariantDto
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                VariantName = variant.VariantName,
                SKU = variant.SKU,
                Price = variant.Price,
                IsEnabled = variant.IsEnabled,
                Attributes = variant.Attributes.Select(a => new ProductVariantAttributeDto
                {
                    Id = a.Id,
                    VariationId = a.VariationId,
                    VariationName = a.Variation.Name,
                    VariationOptionId = a.VariationOptionId,
                    VariationOptionName = a.VariationOption.Name,
                    VariationOptionValue = a.VariationOption.Value
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateVariant(int id, [FromBody] UpdateProductVariantDto dto)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null)
                return NotFound();

            variant.VariantName = dto.VariantName;
            variant.Description = dto.Description;
            variant.ImageUrl = dto.ImageUrl;
            variant.Price = dto.Price;
            variant.BarcodeSKU = dto.BarcodeSKU;
            variant.BarcodeSKU2 = dto.BarcodeSKU2;
            variant.BarcodeSKU3 = dto.BarcodeSKU3;
            variant.BarcodeSKU4 = dto.BarcodeSKU4;
            variant.SKU = dto.SKU;
            variant.IsEnabled = dto.IsEnabled;
            variant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteVariant(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null)
                return NotFound();

            variant.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
