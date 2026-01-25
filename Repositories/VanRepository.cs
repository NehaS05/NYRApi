using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class VanRepository : GenericRepository<Van>, IVanRepository
    {
        public VanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Van?> GetByVanNumberAsync(string vanNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.VanNumber == vanNumber);
        }

        public async Task<IEnumerable<Van>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();
            return await _dbSet.Where(v =>
                v.VanNumber.ToLower().Contains(searchTerm) ||
                v.VanName.ToLower().Contains(searchTerm) ||
                v.DefaultDriverName.ToLower().Contains(searchTerm)
            ).ToListAsync();
        }

        public async Task<IEnumerable<Van>> GetByDriverIdAsync(int driverId)
        {
            return await _dbSet
                .Where(v => v.DriverId == driverId && v.IsActive)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Van> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, v => v.VanName);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Van> BuildBaseQuery()
        {
            return _dbSet
                .Include(v => v.Driver)
                .Where(v => v.IsActive);
        }

        private IQueryable<Van> ApplySearchFilter(IQueryable<Van> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(v =>
                EF.Functions.Like(v.VanName, $"%{search}%") ||
                EF.Functions.Like(v.VanNumber, $"%{search}%") ||
                EF.Functions.Like(v.DefaultDriverName, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<Van, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Van, object>>>
            {
                { "name", v => v.VanName },
                { "vannumber", v => v.VanNumber },
                { "drivername", v => v.DefaultDriverName },
                { "createdat", v => v.CreatedAt }
            };
        }
    }
}


