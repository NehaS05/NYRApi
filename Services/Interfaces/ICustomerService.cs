using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<PagedResultDto<CustomerDto>> GetCustomersPagedAsync(PaginationParamsDto paginationParams);
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
        Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto);
        Task<bool> DeleteCustomerAsync(int id);
        Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm);
    }
}
