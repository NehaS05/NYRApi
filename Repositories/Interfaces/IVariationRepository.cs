using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IVariationRepository : IGenericRepository<Variation>
    {
        Task<Variation?> GetByNameAsync(string name);
        Task<IEnumerable<Variation>> GetActiveVariationsAsync();
        Task<Variation?> GetByIdWithOptionsAsync(int id);
        Task<IEnumerable<Variation>> GetAllWithOptionsAsync();
        Task<(IEnumerable<Variation> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}
