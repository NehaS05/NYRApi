using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    }
}
