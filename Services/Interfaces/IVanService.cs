using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IVanService
    {
        Task<IEnumerable<VanDto>> GetAllAsync();
        Task<PagedResultDto<VanDto>> GetVansPagedAsync(PaginationParamsDto paginationParams);
        Task<VanDto?> GetByIdAsync(int id);
        Task<VanDto> CreateAsync(CreateVanDto dto);
        Task<VanDto?> UpdateAsync(int id, UpdateVanDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<VanDto>> SearchAsync(string searchTerm);
    }
}


