using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class LocationUnlistedInventoryRepository : GenericRepository<LocationUnlistedInventory>, ILocationUnlistedInventoryRepository
    {
        public LocationUnlistedInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LocationUnlistedInventory>> GetByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<LocationUnlistedInventory?> GetByBarcodeAndLocationAsync(string barcodeNo, int locationId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => l.BarcodeNo == barcodeNo && l.LocationId == locationId);
        }

        public override async Task<IEnumerable<LocationUnlistedInventory>> GetAllAsync()
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .ToListAsync();
        }

        public override async Task<LocationUnlistedInventory?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}