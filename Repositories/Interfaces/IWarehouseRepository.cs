using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IWarehouseRepository : IGenericRepository<Warehouse>
    {
        Task<IEnumerable<Warehouse>> SearchAsync(string searchTerm);
        Task<(IEnumerable<Warehouse> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}


