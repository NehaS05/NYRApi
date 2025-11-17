using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface IRequestSupplyRepository : IGenericRepository<RequestSupply>
    {
        Task<IEnumerable<RequestSupply>> GetAllWithItemsAsync();
        Task<RequestSupply?> GetByIdWithItemsAsync(int id);
        Task<IEnumerable<RequestSupply>> GetByStatusAsync(string status);
        Task<bool> DeleteAsync(int id);
    }
}
