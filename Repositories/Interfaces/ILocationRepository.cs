using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<IEnumerable<Location>> GetLocationsByCustomerAsync(int customerId);
        Task<Location?> GetLocationWithCustomerAsync(int id);
        Task<IEnumerable<Location>> SearchLocationsAsync(string searchTerm);
        Task<IEnumerable<Location>> GetLocationsWithoutScannersAsync();
        Task<Location?> GetLocationByScannerSerialNoAsync(string serialNo);
        Task<(IEnumerable<Location> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}
