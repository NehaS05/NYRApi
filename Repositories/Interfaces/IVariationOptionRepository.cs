using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IVariationOptionRepository : IGenericRepository<VariationOption>
    {
        Task<IEnumerable<VariationOption>> GetByVariationIdAsync(int variationId);
    }
}
