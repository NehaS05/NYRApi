using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

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

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, p => p.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Product> BuildBaseQuery()
        {
            return _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(p => p.IsActive);
        }

        private IQueryable<Product> ApplySearchFilter(IQueryable<Product> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(p =>
                EF.Functions.Like(p.Name, $"%{search}%") ||
                EF.Functions.Like(p.Category.Name, $"%{search}%") ||
                EF.Functions.Like(p.Brand.Name, $"%{search}%") ||
                EF.Functions.Like(p.Supplier.Name, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<Product, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Product, object>>>
            {
                { "name", p => p.Name },
                { "categoryname", p => p.Category.Name },
                { "brandname", p => p.Brand.Name },
                { "suppliername", p => p.Supplier.Name },
                { "createdat", p => p.CreatedAt }
            };
        }
    }
}
