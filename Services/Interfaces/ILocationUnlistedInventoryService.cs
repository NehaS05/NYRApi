using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ILocationUnlistedInventoryService
    {
        Task<IEnumerable<LocationUnlistedInventoryDto>> GetAllAsync();
        Task<LocationUnlistedInventoryDto?> GetByIdAsync(int id);
        Task<IEnumerable<LocationUnlistedInventoryDto>> GetByLocationIdAsync(int locationId);
        Task<LocationUnlistedInventoryDto?> GetByBarcodeAndLocationAsync(string barcodeNo, int locationId);
        Task<LocationUnlistedInventoryDto> CreateAsync(CreateLocationUnlistedInventoryDto createDto);
        Task<LocationUnlistedInventoryDto?> UpdateAsync(int id, UpdateLocationUnlistedInventoryDto updateDto);
        Task<bool> DeleteAsync(int id, int userId);
    }
}