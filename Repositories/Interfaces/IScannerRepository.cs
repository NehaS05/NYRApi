using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IScannerRepository : IGenericRepository<Scanner>
    {
        Task<Scanner?> GetScannerWithLocationAsync(int id);
        Task<IEnumerable<Scanner>> GetScannersByLocationAsync(int locationId);
        Task<IEnumerable<Scanner>> SearchScannersAsync(string searchTerm);
        Task<Scanner?> GetBySerialNoAsync(string serialNo);
        Task<Scanner?> GetByRefreshTokenAsync(string refreshToken);
        Task<(IEnumerable<Scanner> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}

