using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;

namespace NYR.API.Repositories
{
    public class RouteRepository : GenericRepository<Routes>, IRouteRepository
    {
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Routes>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                    .ThenInclude(rs => rs.Warehouse)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Location)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Customer)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.Product)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.Variation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.VariationOption)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.FollowupRequest)
                .OrderByDescending(r => r.DeliveryDate)
                .ToListAsync();
        }

        public async Task<Routes?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.RouteStops.Where(x => x.IsActive).OrderBy(s => s.StopOrder))
                    .ThenInclude(rs => rs.Location)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Customer)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.Product)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.Variation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.VariationOption)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.FollowupRequest)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Routes>> GetByLocationIdAsync(int locationId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Location)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Customer)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.Product)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.Variation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.VariationOption)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.FollowupRequest)
                .Where(r => r.RouteStops.Any(rs => rs.LocationId == locationId) && r.IsActive)
                .OrderByDescending(r => r.DeliveryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Routes>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Location)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Customer)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.Product)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.Variation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.VariationOption)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.FollowupRequest)
                .Where(r => r.UserId == userId && r.IsActive)
                .OrderByDescending(r => r.DeliveryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Routes>> GetByDeliveryDateAsync(DateTime deliveryDate)
        {
            var startDate = deliveryDate.Date;
            var endDate = startDate.AddDays(1);

            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Location)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Customer)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.Product)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.Variation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.VariationOption)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.FollowupRequest)
                .Where(r => r.DeliveryDate >= startDate && r.DeliveryDate < endDate && r.IsActive)
                .OrderBy(r => r.DeliveryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Routes>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Location)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.Customer)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.Product)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.Variation)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.RestockRequest)
                        .ThenInclude(rr => rr!.Items)
                            .ThenInclude(item => item.ProductVariant)
                                .ThenInclude(pv => pv!.Attributes)
                                    .ThenInclude(a => a.VariationOption)
                .Include(r => r.RouteStops)
                    .ThenInclude(rs => rs.FollowupRequest)
                .Where(r => r.Status == status && r.IsActive)
                .OrderByDescending(r => r.DeliveryDate)
                .ToListAsync();
        }

        // New method for simplified route summary response
        public async Task<IEnumerable<RouteSummaryDto>> GetAllRoutesSummaryAsync()
        {
            var sql = @"
                SELECT 
                    r.Id,
                    r.UserId,
                    u.Name as UserName,
                    r.DeliveryDate,
                    r.Status,
                    r.IsActive,
                    r.CreatedAt,
                    ISNULL(u.WarehouseId, 0) as WarehouseId,
                    ISNULL(w.Name, '') as WarehouseName,
                    COUNT(DISTINCT rs.LocationId) as RouteStops
                FROM Routes r
                INNER JOIN Users u ON r.UserId = u.Id
                LEFT JOIN Warehouses w ON u.WarehouseId = w.Id
                LEFT JOIN RouteStops rs ON r.Id = rs.RouteId AND rs.IsActive = 1
                WHERE r.IsActive = 1
                GROUP BY r.Id, r.UserId, u.Name, r.DeliveryDate, r.Status, r.IsActive, r.CreatedAt, u.WarehouseId, w.Name
                ORDER BY r.DeliveryDate DESC";

            return await _context.Database.SqlQueryRaw<RouteSummaryDto>(sql).ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var route = await GetByIdAsync(id);
            if (route == null)
                return false;

            route.IsActive = false;
            await UpdateAsync(route);
            return true;
        }
    }
}
