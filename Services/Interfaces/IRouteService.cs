using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<RouteDto?> GetRouteByIdAsync(int id);
        Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto);
        Task<RouteDto?> UpdateRouteAsync(int id, UpdateRouteDto updateRouteDto);
        Task<bool> DeleteRouteAsync(int id);
        Task<IEnumerable<RouteDto>> GetRoutesByLocationIdAsync(int locationId);
        Task<IEnumerable<RouteDto>> GetRoutesByUserIdAsync(int userId);
        Task<IEnumerable<RouteDto>> GetRoutesByDeliveryDateAsync(DateTime deliveryDate);
        Task<RouteDto?> UpdateRouteStatusAsync(int id, string status);
        Task<IEnumerable<RouteDto>> GetRoutesByStatusAsync(string status);
        Task<IEnumerable<RouteSummaryDto>> GetAllRoutesSummaryAsync();
    }
}
