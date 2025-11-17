using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IRequestSupplyItemRepository : IGenericRepository<RequestSupplyItem>
    {
        Task<IEnumerable<RequestSupplyItem>> GetByRequestSupplyIdAsync(int requestSupplyId);
        Task<bool> DeleteAsync(int id);
    }
}
