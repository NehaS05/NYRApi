using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetActiveRolesAsync();
    }
}
