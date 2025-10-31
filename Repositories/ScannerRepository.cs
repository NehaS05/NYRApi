using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class ScannerRepository : GenericRepository<Scanner>, IScannerRepository
    {
        public ScannerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Scanner?> GetScannerWithLocationAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Scanner>> GetScannersByLocationAsync(int locationId)
        {
            return await _dbSet
                .Include(s => s.Location)
                .Where(s => s.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scanner>> SearchScannersAsync(string searchTerm)
        {
            return await _dbSet
                .Include(s => s.Location)
                .Where(s => s.ScannerId.Contains(searchTerm) ||
                           s.ScannerName.Contains(searchTerm) ||
                           s.ScannerPIN.Contains(searchTerm) ||
                           s.Location.LocationName.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Scanner?> GetByScannerIdAsync(string scannerId)
        {
            return await _dbSet
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.ScannerId == scannerId);
        }

        public override async Task<IEnumerable<Scanner>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Location)
                .ToListAsync();
        }

        public override async Task<Scanner?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}

