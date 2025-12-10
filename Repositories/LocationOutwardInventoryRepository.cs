using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class LocationOutwardInventoryRepository : GenericRepository<LocationOutwardInventory>, ILocationOutwardInventoryRepository
    {
        public LocationOutwardInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LocationOutwardInventory>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<LocationOutwardInventory?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LocationOutwardInventory>> GetByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.LocationId == locationId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LocationOutwardInventory>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.ProductId == productId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LocationOutwardInventory>> GetActiveByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.LocationId == locationId && l.IsActive)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<LocationOutwardInventory?> GetByLocationAndProductAsync(int locationId, int productId, int? productVariantId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => 
                    l.LocationId == locationId && 
                    l.ProductId == productId &&
                    l.ProductVariantId == productVariantId &&
                    l.IsActive);
        }
    }
}