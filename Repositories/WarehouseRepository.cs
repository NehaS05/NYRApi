using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Warehouse>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();
            return await _dbSet.Where(w =>
                w.Name.ToLower().Contains(searchTerm) ||
                w.AddressLine1.ToLower().Contains(searchTerm) ||
                (w.AddressLine2 != null && w.AddressLine2.ToLower().Contains(searchTerm)) ||
                w.City.ToLower().Contains(searchTerm) ||
                w.State.ToLower().Contains(searchTerm) ||
                w.ZipCode.ToLower().Contains(searchTerm)
            ).ToListAsync();
        }
    }
}


