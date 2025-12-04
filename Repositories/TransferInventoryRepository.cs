using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class TransferInventoryRepository : GenericRepository<TransferInventory>, ITransferInventoryRepository
    {
        public TransferInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TransferInventory>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(t => t.Customer)
                .Include(t => t.Location)
                .Include(t => t.Items)
                    .ThenInclude(i => i.Product)
                .Include(t => t.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.Variation)
                .Include(t => t.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.VariationOption)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TransferInventory?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Customer)
                .Include(t => t.Location)
                .Include(t => t.Items)
                    .ThenInclude(i => i.Product)
                .Include(t => t.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.Variation)
                .Include(t => t.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.VariationOption)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TransferInventoryItem>> GetItemsByTransferIdAsync(int transferId)
        {
            return await _context.Set<TransferInventoryItem>()
                .Include(i => i.Product)
                .Include(i => i.ProductVariant)
                    .ThenInclude(pv => pv!.Attributes)
                        .ThenInclude(a => a.Variation)
                .Include(i => i.ProductVariant)
                    .ThenInclude(pv => pv!.Attributes)
                        .ThenInclude(a => a.VariationOption)
                .Where(i => i.TransferInventoryId == transferId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransferInventory>> GetByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(t => t.Customer)
                .Include(t => t.Location)
                .Include(t => t.Items)
                    .ThenInclude(i => i.Product)
                .Include(t => t.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.Variation)
                .Include(t => t.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.VariationOption)
                .Where(t => t.LocationId == locationId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
