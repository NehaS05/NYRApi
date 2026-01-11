using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductInventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductInventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("product/{productId}/variants-with-inventory")]
        public async Task<ActionResult> GetProductVariantsWithInventory(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound();

            var variants = product.Variants.Where(v => v.IsActive).Select(v => new
            {
                v.Id,
                v.VariantName,
                v.SKU,
                v.Price,
                v.IsEnabled,
                Attributes = v.Attributes.Select(a => new
                {
                    a.Id,
                    a.VariationId,
                    VariationName = a.Variation.Name,
                    a.VariationOptionId,
                    VariationOptionName = a.VariationOption.Name,
                    VariationOptionValue = a.VariationOption.Value
                }).ToList(),
                WarehouseInventory = _context.WarehouseInventories
                    .Where(wi => wi.ProductVariantId == v.Id && wi.IsActive)
                    .Select(wi => new
                    {
                        wi.WarehouseId,
                        WarehouseName = wi.Warehouse.Name,
                        wi.Quantity
                    }).ToList(),
                TotalStock = _context.WarehouseInventories
                    .Where(wi => wi.ProductVariantId == v.Id && wi.IsActive)
                    .Sum(wi => wi.Quantity)
            }).ToList();

            return Ok(new
            {
                ProductId = product.Id,
                ProductName = product.Name,
                IsUniversal = product.IsUniversal,
                Variants = variants
            });
        }

        [HttpGet("warehouse/{warehouseId}/products-with-variants")]
        public async Task<ActionResult> GetWarehouseProductsWithVariants(int warehouseId)
        {
            var inventory = await _context.WarehouseInventories
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv!.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv!.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(wi => wi.WarehouseId == warehouseId && wi.IsActive)
                .ToListAsync();

            var grouped = inventory.GroupBy(wi => wi.ProductId).Select(g => new
            {
                ProductId = g.Key,
                ProductName = g.First().Product.Name,
                ProductSKU = g.FirstOrDefault(wi => wi.ProductVariant != null)?.ProductVariant?.BarcodeSKU ?? string.Empty,
                Variants = g.Select(wi => new
                {
                    InventoryId = wi.Id,
                    VariantId = wi.ProductVariantId,
                    VariantName = wi.ProductVariant?.VariantName,
                    Quantity = wi.Quantity,
                    Attributes = wi.ProductVariant?.Attributes.Select(a => new
                    {
                        VariationName = a.Variation.Name,
                        OptionName = a.VariationOption.Name
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(grouped);
        }
    }
}
