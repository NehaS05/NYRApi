using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SpokeController : ControllerBase
    {
        private readonly ISpokeApiService _spokeApiService;
        private readonly IRouteService _routeService;

        public SpokeController(ISpokeApiService spokeApiService, IRouteService routeService)
        {
            _spokeApiService = spokeApiService;
            _routeService = routeService;
        }

        [HttpPost("optimize-route")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<SpokeRouteOptimizationResponse>> OptimizeRoute([FromRoute] string request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _spokeApiService.OptimizeRouteAsync(request);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("optimize-route/{routeId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<SpokeRouteOptimizationResponse>> OptimizeExistingRoute(string routeId)
        {
            //// Get the route from database
            //var route = await _routeService.GetRouteByIdAsync(routeId);
            //if (route == null)
            //    return NotFound("Route not found");

            ////Convert route stops to Spoke format(ordered by StopOrder)
            //var spokeRequest = new SpokeRouteOptimizationRequest
            //{
            //    StartTime = route.DeliveryDate,
            //    Stops = route.RouteStops
            //        .OrderBy(s => s.StopOrder)
            //        .Select(stop => new SpokeStop
            //        {
            //            Id = stop.Id.ToString(),
            //            Location = new SpokeLocation
            //            {
            //                Address = stop.Address
            //            },
            //            Notes = stop.Notes,
            //            ServiceTime = 15 // Default service time in minutes
            //        }).ToList()
            //};

            var result = await _spokeApiService.OptimizeRouteAsync(routeId);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("create-plan")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<SpokePlanResponse>> CreatePlan([FromBody] CreateSpokePlanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _spokeApiService.CreatePlanAsync(request);
                
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("import-stops/{planId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<SpokeStopsBatchImportResponse>> ImportStopsToRoute(string planId, [FromBody] List<SpokeStopImport> stops)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //// Get the route from database
            //var route = await _routeService.GetRouteByIdAsync(routeId);
            //if (route == null)
            //    return NotFound("Route not found");

            try
            {
                var result = await _spokeApiService.ImportStopsAsync(planId.ToString(), stops);
                
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("update-stop-status/{routeId}/{stopId}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult> UpdateStopStatus(int routeId, int stopId, [FromBody] UpdateStopStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var update = new SpokeTrackingUpdate
            {
                RouteId = routeId.ToString(),
                StopId = stopId.ToString(),
                Status = request.Status,
                Timestamp = DateTime.UtcNow,
                Notes = request.Notes
            };

            var result = await _spokeApiService.UpdateTrackingAsync(update);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("tracking")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<SpokeTrackingResponse>> UpdateTracking([FromBody] SpokeTrackingUpdate update)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _spokeApiService.UpdateTrackingAsync(update);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("route/{routeId}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<SpokeOptimizedRoute>> GetRouteDetails(string routeId)
        {
            var result = await _spokeApiService.GetRouteDetailsAsync(routeId);
            
            if (result != null)
                return Ok(result);
            else
                return NotFound("Route not found");
        }

        [HttpDelete("route/{routeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelRoute(string routeId)
        {
            var result = await _spokeApiService.CancelRouteAsync(routeId);
            
            if (result)
                return NoContent();
            else
                return BadRequest("Failed to cancel route");
        }

        [HttpGet("drivers")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<GetDriversResponse>> GetDrivers()
        {
            var result = await _spokeApiService.GetDriversAsync();
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("plans/{planId}/stops")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<GetPlanStopsResponse>> GetPlanStops(string planId)
        {
            var result = await _spokeApiService.GetPlanStopsAsync(planId);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
