using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Supplier?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _dbSet
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, s => s.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Supplier> BuildBaseQuery()
        {
            return _dbSet
                .Where(s => s.IsActive);
        }

        private IQueryable<Supplier> ApplySearchFilter(IQueryable<Supplier> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(s =>
                EF.Functions.Like(s.Name, $"%{search}%") ||
                (s.ContactPerson != null && EF.Functions.Like(s.ContactPerson, $"%{search}%")) ||
                EF.Functions.Like(s.Email, $"%{search}%") ||
                EF.Functions.Like(s.PhoneNumber, $"%{search}%") ||
                (s.Address != null && EF.Functions.Like(s.Address, $"%{search}%")));
        }

        private Dictionary<string, Expression<Func<Supplier, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Supplier, object>>>
            {
                { "name", s => s.Name },
                { "contactname", s => s.ContactPerson ?? "" },
                { "address", s => s.Address ?? "" },
                { "phonenumber", s => s.PhoneNumber },
                { "createdat", s => s.CreatedAt }
            };
        }
    }
}
