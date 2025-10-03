using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
        Task<IEnumerable<Product>> GetProductsBySupplierAsync(int supplierId);
        Task<IEnumerable<Product>> GetCatalogueProductsAsync();
        Task<IEnumerable<Product>> GetUniversalProductsAsync();
        Task<Product?> GetByBarcodeAsync(string barcode);
    }
}
