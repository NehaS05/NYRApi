using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ISpokeApiService
    {
        Task<SpokeRouteOptimizationResponse> OptimizeRouteAsync(string routeId);
        Task<SpokeTrackingResponse> UpdateTrackingAsync(SpokeTrackingUpdate update);
        Task<SpokeOptimizedRoute?> GetRouteDetailsAsync(string routeId);
        Task<bool> CancelRouteAsync(string routeId);
        Task<SpokePlanResponse> CreatePlanAsync(CreateSpokePlanRequest request);
        Task<SpokeStopsBatchImportResponse> ImportStopsAsync(string planId, List<SpokeStopImport> stops);
        Task<GetDriversResponse> GetDriversAsync();
        Task<GetPlanStopsResponse> GetPlanStopsAsync(string planId);
    }
}
