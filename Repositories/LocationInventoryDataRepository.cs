using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class LocationInventoryDataRepository : GenericRepository<LocationInventoryData>, ILocationInventoryDataRepository
    {
        public LocationInventoryDataRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LocationInventoryData>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<LocationInventoryData?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LocationInventoryData>> GetByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.LocationId == locationId)
                .OrderBy(l => l.Product.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<LocationInventoryData>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.ProductId == productId)
                .OrderBy(l => l.Location.LocationName)
                .ToListAsync();
        }

        public async Task<LocationInventoryData?> GetByLocationAndProductAsync(int locationId, int productId, int? productVariantId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => 
                    l.LocationId == locationId && 
                    l.ProductId == productId &&
                    l.ProductVariantId == productVariantId);
        }

        public async Task<LocationInventoryData?> GetByLocationAndProductVariationNameAsync(int locationId, int productId, string? variationName)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l =>
                    l.LocationId == locationId &&
                    l.ProductId == productId &&
                    l.VariationName == variationName);
        }
    }
}
