using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, ApplicationDbContext context, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            customers = customers.Where(x => x.IsActive == true);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<PagedResultDto<CustomerDto>> GetCustomersPagedAsync(PaginationParamsDto paginationParams)
        {
            PaginationServiceHelper.NormalizePaginationParams(paginationParams);

            var (items, totalCount) = await _customerRepository.GetPagedAsync(paginationParams);
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(items);

            return PaginationServiceHelper.CreatePagedResult(customerDtos, totalCount, paginationParams);
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

            // Check if there are locations associated with this customer
            var hasLocationsQuery = _context.Locations
                .Where(l => l.CustomerId == id && l.IsActive);
            
            var hasLocations = await hasLocationsQuery.AnyAsync();
            
            if (hasLocations)
            {
                // Check if any of these locations have active scanners
                var hasActiveScannersQuery = _context.Scanners
                    .Where(s => s.Location.CustomerId == id && s.IsActive);
                
                var hasActiveScanners = await hasActiveScannersQuery.AnyAsync();
                
                if (hasActiveScanners)
                {
                    // Soft delete: deactivate customer, locations, and scanners
                    customer.IsActive = false;
                    customer.UpdatedAt = DateTime.UtcNow;
                    await _customerRepository.UpdateAsync(customer);
                    
                    // Deactivate associated locations
                    var locations = await hasLocationsQuery.ToListAsync();
                    foreach (var location in locations)
                    {
                        location.IsActive = false;
                        location.UpdatedAt = DateTime.UtcNow;
                    }
                    
                    // Deactivate associated scanners
                    var scanners = await hasActiveScannersQuery.ToListAsync();
                    foreach (var scanner in scanners)
                    {
                        scanner.IsActive = false;
                        scanner.UpdatedAt = DateTime.UtcNow;
                    }
                    
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // No active scanners, but has locations - soft delete customer and locations
                    customer.IsActive = false;
                    customer.UpdatedAt = DateTime.UtcNow;
                    await _customerRepository.UpdateAsync(customer);
                    
                    var locations = await hasLocationsQuery.ToListAsync();
                    foreach (var location in locations)
                    {
                        location.IsActive = false;
                        location.UpdatedAt = DateTime.UtcNow;
                    }
                    
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // No locations, safe to hard delete
                await _customerRepository.DeleteAsync(customer);
            }
            
            return true;
        }

        public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
        {
            var customers = await _customerRepository.SearchCustomersAsync(searchTerm);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}
