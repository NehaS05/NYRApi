using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ILocationUnlistedInventoryRepository : IGenericRepository<LocationUnlistedInventory>
    {
        Task<IEnumerable<LocationUnlistedInventory>> GetByLocationIdAsync(int locationId);
        Task<LocationUnlistedInventory?> GetByBarcodeAndLocationAsync(string barcodeNo, int locationId);
    }
}