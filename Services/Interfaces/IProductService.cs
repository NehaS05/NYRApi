using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetProductsByBrandAsync(int brandId);
        Task<IEnumerable<ProductDto>> GetProductsBySupplierAsync(int supplierId);
        Task<IEnumerable<ProductDto>> GetCatalogueProductsAsync();
        Task<IEnumerable<ProductDto>> GetUniversalProductsAsync();
        Task<ProductDto?> GetProductByBarcodeAsync(string barcode);
    }
}
