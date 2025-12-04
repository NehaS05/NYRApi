using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IWarehouseInventoryRepository : IGenericRepository<WarehouseInventory>
    {
        Task<IEnumerable<WarehouseInventory>> GetByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<WarehouseInventory>> GetByProductIdAsync(int productId);
        Task<WarehouseInventory?> GetByWarehouseAndProductVariantAsync(int warehouseId, int productVariantId);
        Task<IEnumerable<WarehouseInventory>> GetInventoryWithDetailsAsync();
        Task<IEnumerable<WarehouseInventory>> GetInventoryByWarehouseWithDetailsAsync(int warehouseId);
        Task<bool> ExistsByWarehouseAndProductVariantAsync(int warehouseId, int productVariantId);
        Task<Dictionary<int, int>> GetWarehouseProductCountsAsync();
        Task<Dictionary<int, int>> GetWarehouseQuantityTotalsAsync();
    }
}
