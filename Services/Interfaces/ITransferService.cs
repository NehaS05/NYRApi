using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ITransferService
    {
        Task<IEnumerable<TransferDto>> GetAllTransfersAsync();
        Task<PagedResultDto<TransferDto>> GetAllTransfersPagedAsync(PaginationParamsDto paginationParams);
        Task<TransferDto?> GetTransferByIdAsync(int id, string type);
        Task<IEnumerable<TransferDto>> GetTransfersByLocationIdAsync(int locationId);
        Task<IEnumerable<TransferDto>> GetTransfersByCustomerIdAsync(int customerId);
        Task<IEnumerable<TransferDto>> GetTransfersByStatusAsync(string status);
        Task<IEnumerable<TransferDto>> GetTransfersByTypeAsync(string type);
        Task<PagedResultDto<TransferDto>> GetTransfersByTypePagedAsync(string type, PaginationParamsDto paginationParams);
        Task<TransferSummaryDto> GetTransfersSummaryAsync();
        Task<InventoryCountsByDriverDto?> GetInventoryCountsByDriverIdAsync(int driverId);
    }
}
