using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

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

        public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, c => c.CompanyName);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Customer> BuildBaseQuery()
        {
            return _dbSet
                .Include(c => c.Locations)
                .Where(c => c.IsActive);
        }

        private IQueryable<Customer> ApplySearchFilter(IQueryable<Customer> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(c =>
                EF.Functions.Like(c.CompanyName, $"%{search}%") ||
                EF.Functions.Like(c.ContactName, $"%{search}%") ||
                EF.Functions.Like(c.AddressLine1, $"%{search}%") ||
                (c.BusinessPhone != null && EF.Functions.Like(c.BusinessPhone, $"%{search}%")) ||
                (c.MobilePhone != null && EF.Functions.Like(c.MobilePhone, $"%{search}%")));
        }

        private Dictionary<string, Expression<Func<Customer, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Customer, object>>>
            {
                { "companyname", c => c.CompanyName },
                { "contactname", c => c.ContactName },
                { "address", c => c.AddressLine1 },
                { "phonenumber", c => c.BusinessPhone ?? c.MobilePhone ?? "" },
                { "createdat", c => c.CreatedAt }
            };
        }
    }
}
