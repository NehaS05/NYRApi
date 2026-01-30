using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

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
