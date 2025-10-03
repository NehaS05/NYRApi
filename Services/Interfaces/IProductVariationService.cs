using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IProductVariationService
    {
        Task<IEnumerable<ProductVariationDto>> GetAllVariationsAsync();
        Task<ProductVariationDto?> GetVariationByIdAsync(int id);
        Task<ProductVariationDto> CreateVariationAsync(CreateProductVariationDto createVariationDto);
        Task<ProductVariationDto?> UpdateVariationAsync(int id, UpdateProductVariationDto updateVariationDto);
        Task<bool> DeleteVariationAsync(int id);
        Task<IEnumerable<ProductVariationDto>> GetVariationsByProductAsync(int productId);
        Task<IEnumerable<ProductVariationDto>> GetVariationsByTypeAsync(int productId, string variationType);
    }
}
