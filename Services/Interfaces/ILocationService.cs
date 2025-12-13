using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<IEnumerable<LocationDto>> GetAllLocationsWithInventoryAsync();
        Task<LocationDto?> GetLocationByIdAsync(int id);
        Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto);
        Task<LocationDto?> UpdateLocationAsync(int id, UpdateLocationDto updateLocationDto);
        Task<bool> DeleteLocationAsync(int id);
        Task<IEnumerable<LocationDto>> GetLocationsByCustomerAsync(int customerId);
        Task<IEnumerable<LocationDto>> SearchLocationsAsync(string searchTerm);
        Task<IEnumerable<LocationDto>> GetLocationsNeedingFollowUpAsync();
    }
}
