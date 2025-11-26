using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class RestockRequestRepository : GenericRepository<RestockRequest>, IRestockRequestRepository
    {
        public RestockRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RestockRequest>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(rr => rr.Customer)
                .Include(rr => rr.Location)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.Product)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.ProductVariation)
                .Where(rr => rr.IsActive)
                .OrderByDescending(rr => rr.RequestDate)
                .ToListAsync();
        }

        public async Task<RestockRequest?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(rr => rr.Customer)
                .Include(rr => rr.Location)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.Product)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.ProductVariation)
                .FirstOrDefaultAsync(rr => rr.Id == id);
        }

        public async Task<IEnumerable<RestockRequest>> GetByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(rr => rr.Customer)
                .Include(rr => rr.Location)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.Product)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.ProductVariation)
                .Where(rr => rr.LocationId == locationId && rr.IsActive)
                .OrderByDescending(rr => rr.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RestockRequest>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Include(rr => rr.Customer)
                .Include(rr => rr.Location)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.Product)
                .Include(rr => rr.Items)
                    .ThenInclude(item => item.ProductVariation)
                .Where(rr => rr.CustomerId == customerId && rr.IsActive)
                .OrderByDescending(rr => rr.RequestDate)
                .ToListAsync();
        }
    }
}
