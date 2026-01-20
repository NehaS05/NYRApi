using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ILocationInventoryDataRepository : IGenericRepository<LocationInventoryData>
    {
        Task<IEnumerable<LocationInventoryData>> GetAllWithDetailsAsync();
        Task<IEnumerable<IGrouping<int, LocationInventoryData>>> GetAllGroupedByLocationAsync();
        Task<LocationInventoryData?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<LocationInventoryData>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<LocationInventoryData>> GetByProductIdAsync(int productId);
        Task<LocationInventoryData?> GetByLocationAndProductAsync(int locationId, int productId, int? productVariantId);
        Task<LocationInventoryData?> GetByLocationAndProductVariationNameAsync(int locationId, int productId, string? variationName);
    }
}
