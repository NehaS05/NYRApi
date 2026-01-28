using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IVanInventoryRepository : IGenericRepository<VanInventory>
    {
        Task<IEnumerable<VanInventory>> GetByVanIdAsync(int vanId);
        Task<VanInventory?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<VanInventory>> GetAllWithDetailsAsync();
        Task<(IEnumerable<VanWithInventorySummaryDto> Items, int TotalCount)> GetVansWithTransfersPagedAsync(PaginationParamsDto paginationParams);
    }
}
