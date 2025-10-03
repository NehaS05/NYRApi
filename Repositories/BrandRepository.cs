using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(b => b.Name == name);
        }

        public async Task<IEnumerable<Brand>> GetActiveBrandsAsync()
        {
            return await _dbSet
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _dbSet
                .OrderBy(b => b.Name)
                .ToListAsync();
        }
    }
}
