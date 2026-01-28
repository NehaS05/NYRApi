using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IVanInventoryService
    {
        Task<IEnumerable<VanWithInventorySummaryDto>> GetVansWithTransfersAsync();
        Task<PagedResultDto<VanWithInventorySummaryDto>> GetVansWithTransfersPagedAsync(PaginationParamsDto paginationParams);
        Task<IEnumerable<VanInventoryDto>> GetAllTransfersAsync();
        Task<VanInventoryDto?> GetTransferByIdAsync(int id);
        Task<IEnumerable<VanInventoryItemDto>> GetTransferItemsByVanIdAsync(int vanId);
        Task<VanInventoryDto> CreateTransferAsync(CreateVanInventoryDto createDto);
        Task<bool> DeleteTransferAsync(int id);
        Task<IEnumerable<TransferTrackingDto>> GetAllTransfersTrackingAsync();
        Task<TransferTrackingDto?> UpdateTransferStatusAsync(int id, UpdateTransferStatusDto updateDto);
    }
}
