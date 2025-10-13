using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IDriverAvailabilityRepository : IGenericRepository<DriverAvailability>
    {
        Task<IEnumerable<DriverAvailability>> GetByUserIdAsync(int userId);
        Task<DriverAvailability?> GetByUserIdAndDayAsync(int userId, string dayOfWeek);
        Task DeleteByUserIdAsync(int userId);
        Task<IEnumerable<DriverAvailability>> GetActiveByUserIdAsync(int userId);
    }
}
