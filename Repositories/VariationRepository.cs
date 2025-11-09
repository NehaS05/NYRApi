using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class VariationRepository : GenericRepository<Variation>, IVariationRepository
    {
        public VariationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Variation?> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(v => v.Options)
                .FirstOrDefaultAsync(v => v.Name == name);
        }

        public async Task<IEnumerable<Variation>> GetActiveVariationsAsync()
        {
            return await _dbSet
                .Include(v => v.Options.Where(o => o.IsActive))
                .Where(v => v.IsActive)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<Variation?> GetByIdWithOptionsAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Options)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Variation>> GetAllWithOptionsAsync()
        {
            return await _dbSet
                .Include(v => v.Options)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Variation>> GetAllAsync()
        {
            return await _dbSet
                .Include(v => v.Options)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public override async Task<Variation?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Options)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}
