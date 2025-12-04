using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
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
                .Include(r => r.RouteStops.OrderBy(s => s.StopOrder))
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
