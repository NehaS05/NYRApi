using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<IEnumerable<Location>> GetLocationsByCustomerAsync(int customerId);
        Task<Location?> GetLocationWithCustomerAsync(int id);
        Task<IEnumerable<Location>> SearchLocationsAsync(string searchTerm);
        Task<IEnumerable<Location>> GetLocationsWithoutScannersAsync();
    }
}
