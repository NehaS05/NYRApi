using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class RequestSupplyItemRepository : GenericRepository<RequestSupplyItem>, IRequestSupplyItemRepository
    {
        public RequestSupplyItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RequestSupplyItem>> GetByRequestSupplyIdAsync(int requestSupplyId)
        {
            return await _dbSet
                .Include(i => i.Variation)
                    .ThenInclude(vo => vo.Variation)
                .Where(i => i.RequestSupplyId == requestSupplyId && i.IsActive)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item == null)
                return false;

            item.IsActive = false;
            await UpdateAsync(item);
            return true;
        }
    }
}
