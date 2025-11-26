using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class FollowupRequestRepository : GenericRepository<FollowupRequest>, IFollowupRequestRepository
    {
        public FollowupRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FollowupRequest>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(fr => fr.Customer)
                .Include(fr => fr.Location)
                .Where(fr => fr.IsActive)
                .OrderByDescending(fr => fr.FollowupDate)
                .ToListAsync();
        }

        public async Task<FollowupRequest?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(fr => fr.Customer)
                .Include(fr => fr.Location)
                .FirstOrDefaultAsync(fr => fr.Id == id);
        }

        public async Task<IEnumerable<FollowupRequest>> GetByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(fr => fr.Customer)
                .Include(fr => fr.Location)
                .Where(fr => fr.LocationId == locationId && fr.IsActive)
                .OrderByDescending(fr => fr.FollowupDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FollowupRequest>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Include(fr => fr.Customer)
                .Include(fr => fr.Location)
                .Where(fr => fr.CustomerId == customerId && fr.IsActive)
                .OrderByDescending(fr => fr.FollowupDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FollowupRequest>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(fr => fr.Customer)
                .Include(fr => fr.Location)
                .Where(fr => fr.Status == status && fr.IsActive)
                .OrderByDescending(fr => fr.FollowupDate)
                .ToListAsync();
        }
    }
}
