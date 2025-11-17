using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class VariationOptionRepository : GenericRepository<VariationOption>, IVariationOptionRepository
    {
        public VariationOptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<VariationOption>> GetByVariationIdAsync(int variationId)
        {
            return await _dbSet
                .Include(vo => vo.Variation)
                .Where(vo => vo.VariationId == variationId && vo.IsActive)
                .OrderBy(vo => vo.Name)
                .ToListAsync();
        }
    }
}
