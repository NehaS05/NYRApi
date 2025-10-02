using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByAccountNumberAsync(string accountNumber);
        Task<Customer?> GetCustomerWithLocationsAsync(int id);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
    }
}
