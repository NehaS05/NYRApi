using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _dbSet
                .Include(c => c.Locations)
                .FirstOrDefaultAsync(c => c.AccountNumber == accountNumber);
        }

        public async Task<Customer?> GetCustomerWithLocationsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Locations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
        {
            return await _dbSet
                .Where(c => c.CompanyName.Contains(searchTerm) ||
                           c.ContactName.Contains(searchTerm) ||
                           c.AccountNumber.Contains(searchTerm) ||
                           c.Email!.Contains(searchTerm))
                .ToListAsync();
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Locations)
                .ToListAsync();
        }

        public override async Task<Customer?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Locations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
