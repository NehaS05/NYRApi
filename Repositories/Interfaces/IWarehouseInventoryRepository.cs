using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IWarehouseInventoryRepository : IGenericRepository<WarehouseInventory>
    {
        Task<IEnumerable<WarehouseInventory>> GetByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<WarehouseInventory>> GetByProductIdAsync(int productId);
        Task<WarehouseInventory?> GetByWarehouseAndProductVariationAsync(int warehouseId, int productVariationId);
        Task<IEnumerable<WarehouseInventory>> GetInventoryWithDetailsAsync();
        Task<IEnumerable<WarehouseInventory>> GetInventoryByWarehouseWithDetailsAsync(int warehouseId);
        Task<bool> ExistsByWarehouseAndProductVariationAsync(int warehouseId, int productVariationId);
        Task<Dictionary<int, int>> GetWarehouseProductCountsAsync();
        Task<Dictionary<int, int>> GetWarehouseQuantityTotalsAsync();
    }
}
