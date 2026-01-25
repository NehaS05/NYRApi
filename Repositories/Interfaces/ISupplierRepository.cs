using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        Task<Supplier?> GetByEmailAsync(string email);
        Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
        Task<(IEnumerable<Supplier> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}
