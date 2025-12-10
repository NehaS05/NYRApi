using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ILocationOutwardInventoryService
    {
        Task<IEnumerable<LocationOutwardInventoryDto>> GetAllOutwardInventoryAsync();
        Task<LocationOutwardInventoryDto?> GetOutwardInventoryByIdAsync(int id);
        Task<IEnumerable<LocationOutwardInventoryDto>> GetOutwardInventoryByLocationIdAsync(int locationId, bool? last60Minutes = null);
        Task<IEnumerable<LocationOutwardInventoryDto>> GetOutwardInventoryByProductIdAsync(int productId);
        Task<IEnumerable<LocationOutwardInventoryDto>> GetActiveOutwardInventoryByLocationIdAsync(int locationId);
        Task<LocationOutwardInventoryDto> CreateOutwardInventoryAsync(CreateLocationOutwardInventoryDto createDto);
        Task<LocationOutwardInventoryDto?> UpdateOutwardInventoryAsync(int id, UpdateLocationOutwardInventoryDto updateDto);
        Task<bool> DeleteOutwardInventoryAsync(int id, int userId);
        Task<bool> DeactivateOutwardInventoryAsync(int id, int userId);
    }
}