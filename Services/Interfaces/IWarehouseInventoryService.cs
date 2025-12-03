using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IWarehouseInventoryService
    {
        Task<WarehouseInventoryDto> AddInventoryAsync(AddInventoryDto addInventoryDto);
        Task<IEnumerable<WarehouseInventoryDto>> AddBulkInventoryAsync(AddBulkInventoryDto addBulkInventoryDto);
        Task<IEnumerable<WarehouseListDto>> GetWarehouseListAsync();
        Task<IEnumerable<WarehouseInventoryDetailDto>> GetWarehouseInventoryDetailsAsync(int warehouseId);
        Task<WarehouseInventoryDto?> GetInventoryByIdAsync(int id);
        Task<WarehouseInventoryDto?> UpdateInventoryAsync(int id, UpdateInventoryDto updateInventoryDto);
        Task<bool> DeleteInventoryAsync(int id);
        Task<IEnumerable<WarehouseInventoryDto>> GetInventoryByWarehouseAsync(int warehouseId);
        Task<IEnumerable<WarehouseInventoryDto>> GetInventoryByProductAsync(int productId);
        Task<bool> ExistsByWarehouseAndProductVariantAsync(int warehouseId, int? productVariantId);
    }
}
