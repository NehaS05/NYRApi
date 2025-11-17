using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class WarehouseInventoryRepository : GenericRepository<WarehouseInventory>, IWarehouseInventoryRepository
    {
        public WarehouseInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WarehouseInventory>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _dbSet
                .Where(wi => wi.WarehouseId == warehouseId && wi.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariation)
                .ToListAsync();
        }

        public async Task<IEnumerable<WarehouseInventory>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Where(wi => wi.ProductId == productId && wi.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariation)
                .ToListAsync();
        }

        public async Task<WarehouseInventory?> GetByWarehouseAndProductVariationAsync(int warehouseId, int productVariationId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(wi => wi.WarehouseId == warehouseId && 
                                         wi.ProductVariationId == productVariationId && 
                                         wi.IsActive);
        }

        public async Task<IEnumerable<WarehouseInventory>> GetInventoryWithDetailsAsync()
        {
            return await _dbSet
                .Where(wi => wi.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariation)
                .ToListAsync();
        }

        public async Task<IEnumerable<WarehouseInventory>> GetInventoryByWarehouseWithDetailsAsync(int warehouseId)
        {
            return await _dbSet
                .Where(wi => wi.WarehouseId == warehouseId && wi.IsActive)
                .Include(wi => wi.Warehouse)
                .Include(wi => wi.Product)
                .Include(wi => wi.ProductVariation)
                .ToListAsync();
        }

        public async Task<bool> ExistsByWarehouseAndProductVariationAsync(int warehouseId, int productVariationId)
        {
            return await _dbSet
                .AnyAsync(wi => wi.WarehouseId == warehouseId && 
                              wi.ProductVariationId == productVariationId && 
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
    }
}
