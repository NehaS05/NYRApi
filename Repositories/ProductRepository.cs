using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetProductWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(p => p.BrandId == brandId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsBySupplierAsync(int supplierId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(p => p.SupplierId == supplierId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetCatalogueProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(p => p.ShowInCatalogue && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetUniversalProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(p => p.IsUniversal && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            // Search in ProductVariant barcodes instead of Product barcodes
            var variant = await _context.ProductVariants
                .Include(pv => pv.Product)
                    .ThenInclude(p => p.Category)
                .Include(pv => pv.Product)
                    .ThenInclude(p => p.Brand)
                .Include(pv => pv.Product)
                    .ThenInclude(p => p.Supplier)
                .Include(pv => pv.Product)
                    .ThenInclude(p => p.Variants)
                        .ThenInclude(v => v.Attributes)
                            .ThenInclude(a => a.Variation)
                .Include(pv => pv.Product)
                    .ThenInclude(p => p.Variants)
                        .ThenInclude(v => v.Attributes)
                            .ThenInclude(a => a.VariationOption)
                .FirstOrDefaultAsync(pv => pv.BarcodeSKU == barcode || 
                                          pv.BarcodeSKU2 == barcode || 
                                          pv.BarcodeSKU3 == barcode || 
                                          pv.BarcodeSKU4 == barcode);
            
            return variant?.Product;
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
