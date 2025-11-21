using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class RouteStopRepository : GenericRepository<RouteStop>, IRouteStopRepository
    {
        public RouteStopRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RouteStop>> GetByRouteIdAsync(int routeId)
        {
            return await _dbSet
                .Where(rs => rs.RouteId == routeId && rs.IsActive)
                .OrderBy(rs => rs.StopOrder)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var routeStop = await GetByIdAsync(id);
            if (routeStop == null)
                return false;

            routeStop.IsActive = false;
            await UpdateAsync(routeStop);
            return true;
        }
    }
}
