using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ITransferInventoryService
    {
        Task<IEnumerable<TransferInventoryLocationDto>> GetLocationsWithTransfersAsync();
        Task<IEnumerable<TransferInventoryDto>> GetAllTransfersAsync();
        Task<TransferInventoryDto?> GetTransferByIdAsync(int id);
        Task<IEnumerable<TransferInventoryItemDto>> GetTransferItemsByLocationIdAsync(int locationId);
        Task<TransferInventoryDto> CreateTransferAsync(CreateTransferInventoryDto createDto);
        Task<bool> DeleteTransferAsync(int id);
    }
}
