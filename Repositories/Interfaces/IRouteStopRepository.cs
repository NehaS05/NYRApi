using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IRouteStopRepository : IGenericRepository<RouteStop>
    {
        Task<IEnumerable<RouteStop>> GetByRouteIdAsync(int routeId);
        Task<bool> DeleteAsync(int id);
    }
}
