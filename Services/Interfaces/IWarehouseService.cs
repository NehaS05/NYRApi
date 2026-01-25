using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDto>> GetAllAsync();
        Task<PagedResultDto<WarehouseDto>> GetWarehousesPagedAsync(PaginationParamsDto paginationParams);
        Task<WarehouseDto?> GetByIdAsync(int id);
        Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);
        Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<WarehouseDto>> SearchAsync(string searchTerm);
    }
}


