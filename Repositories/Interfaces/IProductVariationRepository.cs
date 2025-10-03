using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IProductVariationRepository : IGenericRepository<ProductVariation>
    {
        Task<IEnumerable<ProductVariation>> GetVariationsByProductAsync(int productId);
        Task<IEnumerable<ProductVariation>> GetVariationsByTypeAsync(int productId, string variationType);
    }
}
