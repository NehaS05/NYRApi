using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IVanRepository : IGenericRepository<Van>
    {
        Task<Van?> GetByVanNumberAsync(string vanNumber);
        Task<IEnumerable<Van>> SearchAsync(string searchTerm);
        Task<IEnumerable<Van>> GetByDriverIdAsync(int driverId);
        Task<(IEnumerable<Van> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}


