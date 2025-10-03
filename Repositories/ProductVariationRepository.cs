using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class ProductVariationRepository : GenericRepository<ProductVariation>, IProductVariationRepository
    {
        public ProductVariationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductVariation>> GetVariationsByProductAsync(int productId)
        {
            return await _dbSet
                .Where(v => v.ProductId == productId && v.IsActive)
                .OrderBy(v => v.VariationType)
                .ThenBy(v => v.VariationValue)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductVariation>> GetVariationsByTypeAsync(int productId, string variationType)
        {
            return await _dbSet
                .Where(v => v.ProductId == productId && v.VariationType == variationType && v.IsActive)
                .OrderBy(v => v.VariationValue)
                .ToListAsync();
        }

        public override async Task<IEnumerable<ProductVariation>> GetAllAsync()
        {
            return await _dbSet
                .Include(v => v.Product)
                .OrderBy(v => v.ProductId)
                .ThenBy(v => v.VariationType)
                .ThenBy(v => v.VariationValue)
                .ToListAsync();
        }

        public override async Task<ProductVariation?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}
