using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Supplier?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _dbSet
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
