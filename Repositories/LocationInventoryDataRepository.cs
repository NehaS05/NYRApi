using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NYR.API.Repositories
{
    public class LocationInventoryDataRepository : GenericRepository<LocationInventoryData>, ILocationInventoryDataRepository
    {
        public LocationInventoryDataRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LocationInventoryData>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(l => l.Location)
                    .ThenInclude(loc => loc.Customer)
                .Include(l => l.Product)
                .Include(l => l.ProductVariant)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .OrderBy(l => l.Location.LocationName)
                .ThenByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<IGrouping<int, LocationInventoryData>>> GetAllGroupedByLocationAsync()
        {
            var data = await _dbSet
                .Include(l => l.Location)
                    .ThenInclude(loc => loc.Customer)
                .Include(l => l.Product)
                //.Include(l => l.ProductVariant)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .OrderBy(l => l.Location.LocationName)
                .ThenByDescending(l => l.CreatedAt)
                .ToListAsync();

            return data.GroupBy(l => l.LocationId);
        }

        public async Task<LocationInventoryData?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Location)
                    .ThenInclude(loc => loc.Customer)
                .Include(l => l.Product)
                .Include(l => l.ProductVariant)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LocationInventoryData>> GetByLocationIdAsync(int locationId)
        {
            if (locationId == 0)
            {
                return await _dbSet
                .Include(l => l.Location)
                //.ThenInclude(loc => loc.Customer)
                .Include(l => l.Product)
                .Include(l => l.ProductVariant)
                //.Include(l => l.CreatedByUser)
                //.Include(l => l.UpdatedByUser)
                //.Where(l => l.LocationId == locationId)
                .OrderBy(l => l.Product.Name)
                .ToListAsync();
            } else
            {
                return await _dbSet
                .Include(l => l.Location)
                .ThenInclude(loc => loc.Customer)
                .Include(l => l.Product)
                .Include(l => l.ProductVariant)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.LocationId == locationId)
                .OrderBy(l => l.Product.Name)
                .ToListAsync();
            }
            
        }

        public async Task<IEnumerable<LocationInventoryData>> GetItemsByLocationIdAsync()
        {
            var sql = @"
            SELECT 
                rr.Id,
                rr.LocationId,
                --la.LocationName,

                rr.ProductId,
                p.Name AS ProductName,

                pv.BarcodeSKU as BarcodeSKU,

                rr.ProductVariantId,
                rr.Quantity,
                rr.VariationName,

                rr.CreatedAt,
                rr.CreatedBy,
                rr.UpdatedBy,
                rr.UpdatedDate

                --cu.Name AS CreatedByUser
                --uu.Name AS UpdatedByUser

            FROM LocationInventoryData rr
            INNER JOIN Locations la 
                ON rr.LocationId = la.Id
            INNER JOIN Products p 
                ON p.Id = rr.ProductId
            LEFT JOIN ProductVariants pv 
                ON pv.Id = rr.ProductVariantId
            --INNER JOIN Users cu WITH (NOLOCK)
            --    ON rr.CreatedBy = cu.Id
            --LEFT JOIN Users uu 
            --    ON rr.UpdatedBy = uu.Id
";
            return await _context.Database.SqlQueryRaw<LocationInventoryData>(sql).ToListAsync();
            //return await _dbSet
            //    .Include(l => l.Location)
            //        //.ThenInclude(loc => loc.Customer)
            //    .Include(l => l.Product)
            //    .Include(l => l.ProductVariant)
            //    .Include(l => l.CreatedByUser)
            //    .Include(l => l.UpdatedByUser)
            //    //.Where(l => l.LocationId == locationId)
            //    .OrderBy(l => l.Product.Name)
            //    .ToListAsync();
        }

        public async Task<(IEnumerable<LocationInventoryData> Items, int TotalCount)> GetByLocationIdPagedAsync(int locationId, PaginationParamsDto paginationParams)
        {
            IQueryable<LocationInventoryData> query = _dbSet
                .Where(l => l.LocationId == locationId)
                .Include(l => l.Product)
                .Include(l => l.ProductVariant);
            query = ApplyLocationInventorySearchFilter(query, paginationParams.Search);

            var totalCount = await query.CountAsync();

            var sortFields = GetLocationInventorySortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, l => l.Product.Name);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private static IQueryable<LocationInventoryData> ApplyLocationInventorySearchFilter(IQueryable<LocationInventoryData> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(l =>
                EF.Functions.Like(l.Product.Name, $"%{search}%") ||
                (l.ProductVariant != null && EF.Functions.Like(l.ProductVariant.BarcodeSKU, $"%{search}%")) ||
                (l.ProductVariant != null && l.ProductVariant.VariantName != null && EF.Functions.Like(l.ProductVariant.VariantName, $"%{search}%")) ||
                (l.VariationName != null && EF.Functions.Like(l.VariationName, $"%{search}%")));
        }

        private static Dictionary<string, Expression<Func<LocationInventoryData, object>>> GetLocationInventorySortFields()
        {
            return new Dictionary<string, Expression<Func<LocationInventoryData, object>>>
            {
                { "productname", l => l.Product.Name },
                { "skucode", l => l.ProductVariant != null ? (l.ProductVariant.BarcodeSKU ?? "") : "" },
                { "variantname", l => (l.ProductVariant != null ? l.ProductVariant.VariantName : l.VariationName) ?? "" },
                { "quantity", l => l.Quantity },
                { "createdat", l => l.CreatedAt }
            };
        }

        public async Task<IEnumerable<LocationInventoryData>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .Where(l => l.ProductId == productId)
                .OrderBy(l => l.Location.LocationName)
                .ToListAsync();
        }

        public async Task<LocationInventoryData?> GetByLocationAndProductAsync(int locationId, int productId, int? productVariantId)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l => 
                    l.LocationId == locationId && 
                    l.ProductId == productId &&
                    l.ProductVariantId == productVariantId);
        }

        public async Task<LocationInventoryData?> GetByLocationAndProductVariationNameAsync(int locationId, int productId, string? variationName)
        {
            return await _dbSet
                .Include(l => l.Location)
                .Include(l => l.Product)
                .Include(l => l.CreatedByUser)
                .Include(l => l.UpdatedByUser)
                .FirstOrDefaultAsync(l =>
                    l.LocationId == locationId &&
                    l.ProductId == productId &&
                    l.VariationName == variationName);
        }

        public async Task<(IEnumerable<LocationInventoryGroupDto> Items, int TotalCount)> GetAllGroupedByLocationPagedAsync(PaginationParamsDto paginationParams)
        {
            var query = BuildGroupedByLocationBaseQuery();
            query = ApplyGroupedByLocationSearchFilter(query, paginationParams.Search);

            var totalCount = await query.CountAsync();

            var sortFields = GetGroupedByLocationSortFields();
            query = query.ApplySorting(paginationParams.SortBy, paginationParams.SortOrder, sortFields, g => g.LocationName);
            query = query.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        private IQueryable<LocationInventoryGroupDto> BuildGroupedByLocationBaseQuery()
        {
            return _dbSet
                .Include(l => l.Location)
                    .ThenInclude(loc => loc.Customer)
                .AsNoTracking()
                .GroupBy(l => new
                {
                    l.LocationId,
                    l.Location!.LocationName,
                    CustomerName = l.Location.Customer != null ? l.Location.Customer.CompanyName : "Unknown Customer",
                    ContactPerson = l.Location.ContactPerson,
                    l.Location.CreatedAt
                })
                .Select(g => new LocationInventoryGroupDto
                {
                    LocationId = g.Key.LocationId,
                    LocationName = g.Key.LocationName ?? "Unknown Location",
                    CustomerName = g.Key.CustomerName ?? "Unknown Customer",
                    ContactPerson = g.Key.ContactPerson ?? string.Empty,
                    // Location entity does not currently expose a location number; leave null.
                    LocationNumber = null,
                    TotalItems = g.Count(),
                    TotalQuantity = g.Sum(item => item.Quantity),
                    CreatedAt = g.Key.CreatedAt
                });
        }

        private IQueryable<LocationInventoryGroupDto> ApplyGroupedByLocationSearchFilter(IQueryable<LocationInventoryGroupDto> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var search = searchTerm.Trim();
            return query.Where(l =>
                EF.Functions.Like(l.LocationName, $"%{search}%") ||
                EF.Functions.Like(l.CustomerName, $"%{search}%") ||
                EF.Functions.Like(l.ContactPerson ?? string.Empty, $"%{search}%") ||
                EF.Functions.Like(l.LocationNumber ?? string.Empty, $"%{search}%"));
        }

        private Dictionary<string, Expression<Func<LocationInventoryGroupDto, object>>> GetGroupedByLocationSortFields()
        {
            return new Dictionary<string, Expression<Func<LocationInventoryGroupDto, object>>>
            {
                { "locationname", l => l.LocationName },
                { "customername", l => l.CustomerName },
                { "contactperson", l => l.ContactPerson ?? string.Empty },
                { "locationnumber", l => l.LocationNumber ?? string.Empty },
                { "createdat", l => l.CreatedAt }
            };
        }
        // Optimized method using raw SQL for getting inventory summary
        public async Task<IEnumerable<LocationInventoryGroupDto>> GetInventoryGroupSummaryAsync()
        {
            var sql = @"
                SELECT * FROM 
                (SELECT 
                    l.Id as LocationId,
                    l.LocationName,
                    c.CompanyName as CustomerName,
                    COUNT(lid.Id) as TotalItems,
                    ISNULL(SUM(lid.Quantity), 0) as TotalQuantity
                FROM Locations l
                INNER JOIN Customers c ON l.CustomerId = c.Id
                LEFT JOIN LocationInventoryData lid ON l.Id = lid.LocationId
                WHERE l.IsActive = 1
                GROUP BY l.Id, l.LocationName, c.CompanyName) AS loc
                where loc.TotalItems > 0
                ORDER BY loc.LocationName";

            return await _context.Database.SqlQueryRaw<LocationInventoryGroupDto>(sql).ToListAsync();
        }
    }
}
