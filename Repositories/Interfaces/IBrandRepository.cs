using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        Task<Brand?> GetByNameAsync(string name);
        Task<IEnumerable<Brand>> GetActiveBrandsAsync();
    }
}
