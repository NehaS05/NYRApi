using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ILocationInventoryDataService
    {
        Task<IEnumerable<LocationInventoryDataDto>> GetAllInventoryAsync();
        Task<IEnumerable<LocationInventoryGroupDto>> GetAllInventoryGroupedByLocationAsync();
        Task<PagedResultDto<LocationInventoryGroupDto>> GetAllInventoryGroupedByLocationPagedAsync(PaginationParamsDto paginationParams);
        Task<LocationInventoryDataDto?> GetInventoryByIdAsync(int id);
        Task<IEnumerable<LocationInventoryDataDto>> GetInventoryByLocationIdAsync(int locationId);
        Task<IEnumerable<LocationInventoryDataDto>> GetInventoryByProductIdAsync(int productId);
        Task<LocationInventoryDataDto> CreateInventoryAsync(CreateLocationInventoryDataDto createDto);
        Task<LocationInventoryDataDto?> UpdateInventoryAsync(int id, UpdateLocationInventoryDataDto updateDto);
        Task<bool> DeleteInventoryAsync(int id);
        Task<LocationInventoryDataDto?> AdjustQuantityAsync(int id, int quantityChange, int userId);
        Task<IEnumerable<ProductVariantInfoDto>> GetVariantInfoBySkuAsync(string skuCode, int locationId, int? userId = null, int? productVariantId = null);
    }
}
