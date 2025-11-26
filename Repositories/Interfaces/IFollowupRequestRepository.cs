using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IFollowupRequestRepository : IGenericRepository<FollowupRequest>
    {
        Task<IEnumerable<FollowupRequest>> GetAllWithDetailsAsync();
        Task<FollowupRequest?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<FollowupRequest>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<FollowupRequest>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<FollowupRequest>> GetByStatusAsync(string status);
    }
}
