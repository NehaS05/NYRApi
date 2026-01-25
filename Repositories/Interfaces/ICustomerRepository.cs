using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByAccountNumberAsync(string accountNumber);
        Task<Customer?> GetCustomerWithLocationsAsync(int id);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
        Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams);
    }
}
