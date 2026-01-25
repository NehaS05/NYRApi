using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class ScannerRepository : GenericRepository<Scanner>, IScannerRepository
    {
        public ScannerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Scanner?> GetScannerWithLocationAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Scanner>> GetScannersByLocationAsync(int locationId)
        {
            return await _dbSet
                .Include(s => s.Location)
                .Where(s => s.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scanner>> SearchScannersAsync(string searchTerm)
        {
            return await _dbSet
                .Include(s => s.Location)
                .Where(s => s.SerialNo.Contains(searchTerm) ||
                           s.ScannerName.Contains(searchTerm) ||
                           s.ScannerPIN.Contains(searchTerm) ||
                           s.Location.LocationName.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Scanner?> GetBySerialNoAsync(string serialNo)
        {
            return await _dbSet
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.SerialNo == serialNo);
        }

        public async Task<Scanner?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
        }

        public override async Task<IEnumerable<Scanner>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Location)
                .ToListAsync();
        }

        public override async Task<Scanner?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<(IEnumerable<Scanner> Items, int TotalCount)> GetPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildBaseQuery();
            query = ApplySearchFilter(query, paginationParams.Search);
            
            var totalCount = await query.CountAsync();

            var sortFields = GetSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, s => s.ScannerName);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<Scanner> BuildBaseQuery()
        {
            return _dbSet
                .Include(s => s.Location)
                .Where(s => s.IsActive);
        }

        private IQueryable<Scanner> ApplySearchFilter(IQueryable<Scanner> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(s =>
                EF.Functions.Like(s.ScannerName, $"%{search}%") ||
                EF.Functions.Like(s.SerialNo, $"%{search}%") ||
                (s.Location != null && EF.Functions.Like(s.Location.LocationName, $"%{search}%")));
        }

        private Dictionary<string, Expression<Func<Scanner, object>>> GetSortFields()
        {
            return new Dictionary<string, Expression<Func<Scanner, object>>>
            {
                { "name", s => s.ScannerName },
                { "serialnumber", s => s.SerialNo },
                { "locationname", s => s.Location != null ? s.Location.LocationName : "" },
                { "createdat", s => s.CreatedAt }
            };
        }
    }
}

