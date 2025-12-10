using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ILocationOutwardInventoryRepository : IGenericRepository<LocationOutwardInventory>
    {
        Task<IEnumerable<LocationOutwardInventory>> GetAllWithDetailsAsync();
        Task<LocationOutwardInventory?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<LocationOutwardInventory>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<LocationOutwardInventory>> GetByProductIdAsync(int productId);
        Task<IEnumerable<LocationOutwardInventory>> GetActiveByLocationIdAsync(int locationId);
        Task<LocationOutwardInventory?> GetByLocationAndProductAsync(int locationId, int productId, int? productVariantId);
    }
}