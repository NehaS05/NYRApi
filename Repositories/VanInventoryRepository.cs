using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class VanInventoryRepository : GenericRepository<VanInventory>, IVanInventoryRepository
    {
        public VanInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<VanInventory>> GetByVanIdAsync(int vanId)
        {
            return await _dbSet
                .Include(vi => vi.Van)
                .Include(vi => vi.Location)
                    .ThenInclude(l => l.Customer)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.Product)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariation)
                .Where(vi => vi.VanId == vanId && vi.IsActive)
                .OrderByDescending(vi => vi.TransferDate)
                .ToListAsync();
        }

        public async Task<VanInventory?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(vi => vi.Van)
                .Include(vi => vi.Location)
                    .ThenInclude(l => l.Customer)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.Product)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariation)
                .FirstOrDefaultAsync(vi => vi.Id == id);
        }

        public async Task<IEnumerable<VanInventory>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(vi => vi.Van)
                .Include(vi => vi.Location)
                    .ThenInclude(l => l.Customer)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.Product)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariation)
                .Where(vi => vi.IsActive)
                .OrderByDescending(vi => vi.TransferDate)
                .ToListAsync();
        }
    }
}
