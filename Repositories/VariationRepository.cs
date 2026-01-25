using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class VariationRepository : GenericRepository<Variation>, IVariationRepository
    {
        public VariationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Variation?> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(v => v.Options)
                .FirstOrDefaultAsync(v => v.Name == name);
        }

        public async Task<IEnumerable<Variation>> GetActiveVariationsAsync()
        {
            return await _dbSet
                .Include(v => v.Options.Where(o => o.IsActive))
                .Where(v => v.IsActive)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<Variation?> GetByIdWithOptionsAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Options)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Variation>> GetAllWithOptionsAsync()
        {
            return await _dbSet
                .Include(v => v.Options)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Variation>> GetAllAsync()
        {
            return await _dbSet
                .Include(v => v.Options)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public override async Task<Variation?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Options)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<(IEnumerable<Variation> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, v => v.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Variation> BuildBaseQuery()
        {
            return _dbSet
                .Include(v => v.Options)
                .Where(v => v.IsActive);
        }

        private IQueryable<Variation> ApplySearchFilter(IQueryable<Variation> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(v =>
                EF.Functions.Like(v.Name, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<Variation, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Variation, object>>>
            {
                { "name", v => v.Name },
                { "createdat", v => v.CreatedAt }
            };
        }
    }
}
