using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class DriverAvailabilityRepository : GenericRepository<DriverAvailability>, IDriverAvailabilityRepository
    {
        public DriverAvailabilityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DriverAvailability>> GetByUserIdAsync(int userId)
        {
            return await _context.DriverAvailabilities
                .Where(da => da.UserId == userId)
                .OrderBy(da => da.DayOfWeek)
                .ToListAsync();
        }

        public async Task<DriverAvailability?> GetByUserIdAndDayAsync(int userId, string dayOfWeek)
        {
            return await _context.DriverAvailabilities
                .FirstOrDefaultAsync(da => da.UserId == userId && da.DayOfWeek == dayOfWeek);
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var availabilities = await _context.DriverAvailabilities
                .Where(da => da.UserId == userId)
                .ToListAsync();

            if (availabilities.Any())
            {
                _context.DriverAvailabilities.RemoveRange(availabilities);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DriverAvailability>> GetActiveByUserIdAsync(int userId)
        {
            return await _context.DriverAvailabilities
                .Where(da => da.UserId == userId && da.IsActive)
                .OrderBy(da => da.DayOfWeek)
                .ToListAsync();
        }
    }
}
