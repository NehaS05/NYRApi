using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IDriverAvailabilityService
    {
        Task<IEnumerable<DriverAvailabilityDto>> GetByUserIdAsync(int userId);
        Task<DriverAvailabilityDto> CreateAsync(CreateDriverAvailabilityDto createDto);
        Task<DriverAvailabilityDto?> UpdateAsync(int id, UpdateDriverAvailabilityDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByUserIdAsync(int userId);
        Task<bool> SaveBulkAsync(DriverAvailabilityBulkDto bulkDto);
        Task<IEnumerable<DriverAvailabilityDto>> GetActiveByUserIdAsync(int userId);
    }
}
