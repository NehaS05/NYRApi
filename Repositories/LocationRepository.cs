using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Location>> GetLocationsByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Where(l => l.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Location?> GetLocationWithCustomerAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> SearchLocationsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Where(l => l.LocationName.Contains(searchTerm) ||
                           l.ContactPerson!.Contains(searchTerm) ||
                           l.Email!.Contains(searchTerm) ||
                           l.City.Contains(searchTerm))
                .ToListAsync();
        }

        public override async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _dbSet
                .Include(l => l.Customer)
                .ToListAsync();
        }

        public override async Task<Location?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}
