using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Warehouse>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();
            return await _dbSet.Where(w =>
                w.Name.ToLower().Contains(searchTerm) ||
                w.AddressLine1.ToLower().Contains(searchTerm) ||
                (w.AddressLine2 != null && w.AddressLine2.ToLower().Contains(searchTerm)) ||
                w.City.ToLower().Contains(searchTerm) ||
                w.State.ToLower().Contains(searchTerm) ||
                w.ZipCode.ToLower().Contains(searchTerm)
            ).ToListAsync();
        }

        public async Task<(IEnumerable<Warehouse> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, w => w.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Warehouse> BuildBaseQuery()
        {
            return _dbSet
                .Where(w => w.IsActive);
        }

        private IQueryable<Warehouse> ApplySearchFilter(IQueryable<Warehouse> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(w =>
                EF.Functions.Like(w.Name, $"%{search}%") ||
                EF.Functions.Like(w.AddressLine1, $"%{search}%") ||
                EF.Functions.Like(w.City, $"%{search}%") ||
                EF.Functions.Like(w.State, $"%{search}%") ||
                EF.Functions.Like(w.ZipCode, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<Warehouse, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Warehouse, object>>>
            {
                { "name", w => w.Name },
                { "addressline1", w => w.AddressLine1 },
                { "city", w => w.City },
                { "state", w => w.State },
                { "zipcode", w => w.ZipCode },
                { "createdat", w => w.CreatedAt }
            };
        }
    }
}


