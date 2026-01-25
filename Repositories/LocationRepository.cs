using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Location>> GetLocationsByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => l.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Location?> GetLocationWithCustomerAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> SearchLocationsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => l.LocationName.Contains(searchTerm) ||
                           l.ContactPerson!.Contains(searchTerm) ||
                           l.Email!.Contains(searchTerm) ||
                           l.City.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocationsWithoutScannersAsync()
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => !_context.Scanners.Any(s => s.LocationId == l.Id))
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .ToListAsync();
        }

        public override async Task<Location?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<(IEnumerable<Location> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, l => l.LocationName);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Location> BuildBaseQuery()
        {
            return _dbSet
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => l.IsActive);
        }

        private IQueryable<Location> ApplySearchFilter(IQueryable<Location> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(l =>
                EF.Functions.Like(l.LocationName, $"%{search}%") ||
                EF.Functions.Like(l.AddressLine1, $"%{search}%") ||
                EF.Functions.Like(l.City, $"%{search}%") ||
                EF.Functions.Like(l.State, $"%{search}%") ||
                EF.Functions.Like(l.ZipCode, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<Location, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Location, object>>>
            {
                { "name", l => l.LocationName },
                { "address", l => l.AddressLine1 },
                { "city", l => l.City },
                { "state", l => l.State },
                { "zipcode", l => l.ZipCode },
                { "createdat", l => l.CreatedAt }
            };
        }

        public async Task<Location?> GetLocationByScannerSerialNoAsync(string serialNo)
        {
            return await _context.Scanners
                .Include(s => s.Location)
                    //.ThenInclude(l => l.Customer)
                //.Include(s => s.Location)
                //    .ThenInclude(l => l.User)
                .Where(s => s.SerialNo == serialNo && s.Location.IsActive)
                .Select(s => s.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
