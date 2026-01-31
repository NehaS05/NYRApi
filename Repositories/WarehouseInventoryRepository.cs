using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class WarehouseInventoryRepository : GenericRepository<WarehouseInventory>, IWarehouseInventoryRepository
    {
        public WarehouseInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WarehouseInventory>> GetByWarehouseIdAsync(int warehouseId)
        {
            var data = await _dbSet
                .Where(wi => wi.WarehouseId == warehouseId && wi.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .ToListAsync();
            return data;
        }

        public async Task<IEnumerable<WarehouseInventory>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Where(wi => wi.ProductId == productId && wi.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .ToListAsync();
        }

        public async Task<WarehouseInventory?> GetByWarehouseAndProductVariantAsync(int warehouseId, int productVariantId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && 
                                         wi.ProductVariantId == productVariantId && 
                                         wi.IsActive);
        }

        public async Task<IEnumerable<WarehouseInventory>> GetInventoryWithDetailsAsync()
        {
            return await _dbSet
                .Where(wi => wi.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .ToListAsync();
        }

        public async Task<IEnumerable<WarehouseInventory>> GetInventoryByWarehouseWithDetailsAsync(int warehouseId)
        {
            //return await _dbSet
            //    .Where(wi => wi.WarehouseId == warehouseId && wi.IsActive)
            //    .Include(wi => wi.Warehouse)
            //    .Include(wi => wi.Product)
            //    .Include(wi => wi.ProductVariant)
            //        .ThenInclude(pv => pv.Attributes)
            //            .ThenInclude(a => a.Variation)
            //    .Include(wi => wi.ProductVariant)
            //        .ThenInclude(pv => pv.Attributes)
            //            .ThenInclude(a => a.VariationOption)
            //    .ToListAsync();
            return await _dbSet
                .Where(wi =>
                    wi.WarehouseId == warehouseId &&
                    wi.IsActive == true &&
                    wi.Product != null && wi.Product.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product).Where(x => x.IsActive)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .ToListAsync();
        }

        public async Task<(IEnumerable<WarehouseInventory> Items, int TotalCount)> GetInventoryByWarehousePagedAsync(int warehouseId, PaginationParamsDto paginationParams)
        {
            IQueryable<WarehouseInventory> query = _dbSet
                .Where(wi => wi.WarehouseId == warehouseId &&
                    wi.IsActive == true && wi.Product.IsActive)
                .Include(wi => wi.Product).Where(x => x.IsActive)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv!.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv!.Attributes)
                        .ThenInclude(a => a.VariationOption);
            query = ApplyWarehouseInventoryDetailSearchFilter(query, paginationParams.Search);

            var totalCount = await query.CountAsync();

            var sortFields = GetWarehouseInventoryDetailSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, wi => wi.Product.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<WarehouseInventory> ApplyWarehouseInventoryDetailSearchFilter(IQueryable<WarehouseInventory> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(wi =>
                EF.Functions.Like(wi.Product.Name, $"%{search}%") ||
                (wi.ProductVariant != null && EF.Functions.Like(wi.ProductVariant.BarcodeSKU, $"%{search}%")) ||
                (wi.ProductVariant != null && wi.ProductVariant.VariantName != null && EF.Functions.Like(wi.ProductVariant.VariantName, $"%{search}%")));
        }

        private Dictionary<string, Expression<Func<WarehouseInventory, object>>> GetWarehouseInventoryDetailSortFields()
        {
            return new Dictionary<string, Expression<Func<WarehouseInventory, object>>>
            {
                { "productname", wi => wi.Product.Name },
                { "skucode", wi => wi.ProductVariant != null ? (wi.ProductVariant.BarcodeSKU ?? "") : "" },
                { "variantname", wi => wi.ProductVariant != null ? (wi.ProductVariant.VariantName ?? "") : "" },
                { "quantity", wi => wi.Quantity },
                { "createdat", wi => wi.CreatedAt }
            };
        }

        public async Task<bool> ExistsByWarehouseAndProductVariantAsync(int warehouseId, int productVariantId)
        {
            return await _dbSet
                .AnyAsync(wi => wi.WarehouseId == warehouseId && 
                              wi.ProductVariantId == productVariantId && 
                              wi.IsActive);
        }

        public async Task<Dictionary<int, int>> GetWarehouseProductCountsAsync()
        {
            return await _dbSet
                .Where(wi => wi.IsActive)
                .GroupBy(wi => wi.WarehouseId)
                .Select(g => new { WarehouseId = g.Key, ProductCount = g.Select(x => x.ProductId).Distinct().Count() })
                .ToDictionaryAsync(x => x.WarehouseId, x => x.ProductCount);
        }

        public async Task<Dictionary<int, int>> GetWarehouseQuantityTotalsAsync()
        {
            return await _dbSet
                .Where(wi => wi.IsActive)
                .GroupBy(wi => wi.WarehouseId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(wi => wi.Quantity));
        }

        public async Task<(IEnumerable<WarehouseListDto> Items, int TotalCount)> GetWarehouseListPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildWarehouseListBaseQuery();
            query = ApplyWarehouseListSearchFilter(query, paginationParams.Search);

            var totalCount = await query.CountAsync();

            var sortFields = GetWarehouseListSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, w => w.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<WarehouseListDto> BuildWarehouseListBaseQuery()
        {
            return _dbSet
                .Where(wi => wi.IsActive && wi.Warehouse.IsActive)
                .GroupBy(wi => new
                {
                    wi.Warehouse.Id,
                    wi.Warehouse.Name,
                    wi.Warehouse.AddressLine1,
                    wi.Warehouse.AddressLine2,
                    wi.Warehouse.City,
                    wi.Warehouse.State,
                    wi.Warehouse.ZipCode,
                    wi.Warehouse.IsActive,
                    wi.Warehouse.CreatedAt
                })
                .Select(g => new WarehouseListDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    AddressLine1 = g.Key.AddressLine1,
                    AddressLine2 = g.Key.AddressLine2,
                    City = g.Key.City,
                    State = g.Key.State,
                    ZipCode = g.Key.ZipCode,
                    IsActive = g.Key.IsActive,
                    TotalProducts = g.Select(x => x.ProductId).Distinct().Count(),
                    TotalQuantity = g.Sum(x => x.Quantity),
                    CreatedAt = g.Key.CreatedAt
                });
        }

        private IQueryable<WarehouseListDto> ApplyWarehouseListSearchFilter(IQueryable<WarehouseListDto> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(w =>
                EF.Functions.Like(w.Name, $"%{search}%") ||
                EF.Functions.Like(w.AddressLine1, $"%{search}%") ||
                EF.Functions.Like(w.City, $"%{search}%") ||
                EF.Functions.Like(w.State, $"%{search}%") ||
                EF.Functions.Like(w.ZipCode, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<WarehouseListDto, object>>> GetWarehouseListSortFields()
        {
            return new Dictionary<string, Expression<Func<WarehouseListDto, object>>>
            {
                { "name", w => w.Name },
                { "addressline1", w => w.AddressLine1 },
                { "city", w => w.City },
                { "state", w => w.State },
                { "zipcode", w => w.ZipCode },
                { "createdat", w => w.CreatedAt }
            };
        }
    }
}
