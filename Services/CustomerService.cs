using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerWithLocationsAsync(id);
            return customer != null ? _mapper.Map<CustomerDto>(customer) : null;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            // Check if account number already exists
            var existingCustomer = await _customerRepository.GetByAccountNumberAsync(createCustomerDto.AccountNumber);
            if (existingCustomer != null)
                throw new ArgumentException("Account number already exists");

            var customer = _mapper.Map<Customer>(createCustomerDto);
            var createdCustomer = await _customerRepository.AddAsync(customer);
            return _mapper.Map<CustomerDto>(createdCustomer);
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return null;

            // Check if account number already exists for another customer
            var existingCustomer = await _customerRepository.GetByAccountNumberAsync(updateCustomerDto.AccountNumber);
            if (existingCustomer != null && existingCustomer.Id != id)
                throw new ArgumentException("Account number already exists");

            _mapper.Map(updateCustomerDto, customer);
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return false;

            await _customerRepository.DeleteAsync(customer);
            return true;
        }

        public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
        {
            var customers = await _customerRepository.SearchCustomersAsync(searchTerm);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}
