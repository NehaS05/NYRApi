using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ITransferInventoryRepository : IGenericRepository<TransferInventory>
    {
        Task<IEnumerable<TransferInventory>> GetAllWithDetailsAsync();
        Task<TransferInventory?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<TransferInventoryItem>> GetItemsByTransferIdAsync(int transferId);
        Task<IEnumerable<TransferInventory>> GetByLocationIdAsync(int locationId);
    }
}
