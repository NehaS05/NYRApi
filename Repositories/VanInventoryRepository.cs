using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class VanInventoryRepository : GenericRepository<VanInventory>, IVanInventoryRepository
    {
        public VanInventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<VanInventory>> GetByVanIdAsync(int vanId)
        {
            return await _dbSet
                .Include(vi => vi.Van)
                .Include(vi => vi.Location)
                    .ThenInclude(l => l!.Customer)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.Product)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.Variation)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.VariationOption)
                .Where(vi => vi.VanId == vanId && vi.IsActive)
                .OrderByDescending(vi => vi.TransferDate)
                .ToListAsync();
        }

        public async Task<VanInventory?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(vi => vi.Van)
                .Include(vi => vi.Location)
                    .ThenInclude(l => l!.Customer)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.Product)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.Variation)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.VariationOption)
                .FirstOrDefaultAsync(vi => vi.Id == id);
        }

        public async Task<IEnumerable<VanInventory>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(vi => vi.Van)
                .Include(vi => vi.Location)
                    .ThenInclude(l => l!.Customer)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.Product)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.Variation)
                .Include(vi => vi.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv!.Attributes)
                            .ThenInclude(a => a.VariationOption)
                .Where(vi => vi.IsActive)
                .OrderByDescending(vi => vi.TransferDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<VanWithInventorySummaryDto> Items, int TotalCount)> GetVansWithTransfersPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildVansWithTransfersBaseQuery();
            query = ApplyVansWithTransfersSearchFilter(query, paginationParams.Search);

            var totalCount = await query.CountAsync();

            var sortFields = GetVansWithTransfersSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, v => v.VanName);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<VanWithInventorySummaryDto> BuildVansWithTransfersBaseQuery()
        {
            return _dbSet
                .Where(vi => vi.IsActive && vi.Van != null && vi.Van.IsActive)
                .GroupBy(vi => new
                {
                    vi.VanId,
                    vi.Van!.VanName,
                    vi.Van.VanNumber,
                    vi.Van.DefaultDriverName,
                    vi.Van.CreatedAt
                })
                .Select(g => new VanWithInventorySummaryDto
                {
                    VanId = g.Key.VanId,
                    VanName = g.Key.VanName,
                    VanNumber = g.Key.VanNumber,
                    DriverName = g.Key.DefaultDriverName,
                    TotalTransfers = g.Count(),
                    TotalItems = g.SelectMany(vi => vi.Items).Sum(item => item.Quantity),
                    CreatedAt = g.Key.CreatedAt
                });
        }

        private IQueryable<VanWithInventorySummaryDto> ApplyVansWithTransfersSearchFilter(IQueryable<VanWithInventorySummaryDto> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(v =>
                EF.Functions.Like(v.VanName, $"%{search}%") ||
                EF.Functions.Like(v.VanNumber, $"%{search}%") ||
                EF.Functions.Like(v.DriverName, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<VanWithInventorySummaryDto, object>>> GetVansWithTransfersSortFields()
        {
            return new Dictionary<string, Expression<Func<VanWithInventorySummaryDto, object>>>
            {
                { "vanname", v => v.VanName },
                { "vannumber", v => v.VanNumber },
                { "drivername", v => v.DriverName },
                { "createdat", v => v.CreatedAt }
            };
        }
    }
}
