using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IRestockRequestRepository : IGenericRepository<RestockRequest>
    {
        Task<IEnumerable<RestockRequest>> GetAllWithDetailsAsync();
        Task<RestockRequest?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<RestockRequest>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<RestockRequest>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<RestockRequest>> GetByLocationIdWithRouteAsync(int locationId);
    }
}
