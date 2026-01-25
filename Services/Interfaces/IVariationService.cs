using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IVariationService
    {
        Task<IEnumerable<VariationDto>> GetAllVariationsAsync();
        Task<PagedResultDto<VariationDto>> GetVariationsPagedAsync(PaginationParamsDto paginationParams);
        Task<VariationDto?> GetVariationByIdAsync(int id);
        Task<VariationDto> CreateVariationAsync(CreateVariationDto createVariationDto);
        Task<VariationDto?> UpdateVariationAsync(int id, UpdateVariationDto updateVariationDto);
        Task<bool> DeleteVariationAsync(int id);
        Task<IEnumerable<VariationDto>> GetActiveVariationsAsync();
    }
}
