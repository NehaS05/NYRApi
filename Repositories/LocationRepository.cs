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
                .Include(l => l.User)
                .Where(l => l.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Location?> GetLocationWithCustomerAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> SearchLocationsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => l.LocationName.Contains(searchTerm) ||
                           l.ContactPerson!.Contains(searchTerm) ||
                           l.Email!.Contains(searchTerm) ||
                           l.City.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocationsWithoutScannersAsync()
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => !_context.Scanners.Any(s => s.LocationId == l.Id))
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .ToListAsync();
        }

        public override async Task<Location?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Location?> GetLocationByScannerSerialNoAsync(string serialNo)
        {
            return await _context.Scanners
                .Include(s => s.Location)
                    //.ThenInclude(l => l.Customer)
                //.Include(s => s.Location)
                //    .ThenInclude(l => l.User)
                .Where(s => s.SerialNo == serialNo && s.Location.IsActive)
                .Select(s => s.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
