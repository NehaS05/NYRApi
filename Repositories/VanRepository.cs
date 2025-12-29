using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class VanRepository : GenericRepository<Van>, IVanRepository
    {
        public VanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Van?> GetByVanNumberAsync(string vanNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.VanNumber == vanNumber);
        }

        public async Task<IEnumerable<Van>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();
            return await _dbSet.Where(v =>
                v.VanNumber.ToLower().Contains(searchTerm) ||
                v.VanName.ToLower().Contains(searchTerm) ||
                v.DefaultDriverName.ToLower().Contains(searchTerm)
            ).ToListAsync();
        }

        public async Task<IEnumerable<Van>> GetByDriverIdAsync(int driverId)
        {
            return await _dbSet
                .Where(v => v.DriverId == driverId && v.IsActive)
                .ToListAsync();
        }
    }
}


