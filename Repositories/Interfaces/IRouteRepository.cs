using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IRouteRepository : IGenericRepository<Routes>
    {
        Task<IEnumerable<Routes>> GetAllWithDetailsAsync();
        Task<Routes?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Routes>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<Routes>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Routes>> GetByDeliveryDateAsync(DateTime deliveryDate);
        Task<bool> DeleteAsync(int id);
    }
}
