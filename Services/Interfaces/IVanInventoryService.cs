using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IVanInventoryService
    {
        Task<IEnumerable<VanWithInventorySummaryDto>> GetVansWithTransfersAsync();
        Task<IEnumerable<VanInventoryDto>> GetAllTransfersAsync();
        Task<VanInventoryDto?> GetTransferByIdAsync(int id);
        Task<IEnumerable<VanInventoryItemDto>> GetTransferItemsByVanIdAsync(int vanId);
        Task<VanInventoryDto> CreateTransferAsync(CreateVanInventoryDto createDto);
        Task<bool> DeleteTransferAsync(int id);
    }
}
