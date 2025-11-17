using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class RequestSupplyRepository : GenericRepository<RequestSupply>, IRequestSupplyRepository
    {
        public RequestSupplyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RequestSupply>> GetAllWithItemsAsync()
        {
            return await _dbSet
                .Include(rs => rs.Items)
                    .ThenInclude(i => i.Variation)
                        .ThenInclude(vo => vo.Variation)
                .OrderByDescending(rs => rs.CreatedAt)
                .ToListAsync();
        }

        public async Task<RequestSupply?> GetByIdWithItemsAsync(int id)
        {
            return await _dbSet
                .Include(rs => rs.Items)
                    .ThenInclude(i => i.Variation)
                        .ThenInclude(vo => vo.Variation)
                .FirstOrDefaultAsync(rs => rs.Id == id);
        }

        public async Task<IEnumerable<RequestSupply>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(rs => rs.Items)
                    .ThenInclude(i => i.Variation)
                        .ThenInclude(vo => vo.Variation)
                .Where(rs => rs.Status == status && rs.IsActive)
                .OrderByDescending(rs => rs.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var requestSupply = await GetByIdAsync(id);
            if (requestSupply == null)
                return false;

            requestSupply.IsActive = false;
            await UpdateAsync(requestSupply);
            return true;
        }
    }
}
