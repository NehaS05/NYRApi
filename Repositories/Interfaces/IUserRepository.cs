using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithDetailsAsync(int id);
        Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
        Task<IEnumerable<User>> GetUsersByCustomerAsync(int customerId);
        Task<IEnumerable<User>> GetUsersByLocationAsync(int locationId);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}
