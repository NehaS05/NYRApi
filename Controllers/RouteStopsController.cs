using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Repositories.Interfaces;
using AutoMapper;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RouteStopsController : ControllerBase
    {
        private readonly IRouteStopRepository _routeStopRepository;
        private readonly IRestockRequestRepository _restockRequestRepository;
        private readonly IFollowupRequestRepository _followupRequestRepository;
        private readonly ILocationInventoryDataRepository _locationInventoryDataRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IMapper _mapper;

        public RouteStopsController(
            IRouteStopRepository routeStopRepository,
            IRestockRequestRepository restockRequestRepository,
            IFollowupRequestRepository followupRequestRepository,
            ILocationInventoryDataRepository locationInventoryDataRepository,
            IRouteRepository routeRepository,
            IMapper mapper)
        {
            _routeStopRepository = routeStopRepository;
            _restockRequestRepository = restockRequestRepository;
            _followupRequestRepository = followupRequestRepository;
            _locationInventoryDataRepository = locationInventoryDataRepository;
            _routeRepository = routeRepository;
            _mapper = mapper;
        }

        [HttpGet("by-route/{routeId}")]
        public async Task<ActionResult<IEnumerable<RouteStopDto>>> GetStopsByRoute(int routeId)
        {
            var stops = await _routeStopRepository.GetByRouteIdAsync(routeId);
            var stopDtos = _mapper.Map<IEnumerable<RouteStopDto>>(stops);
            return Ok(stopDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteStopDto>> GetStop(int id)
        {
            var stop = await _routeStopRepository.GetByIdAsync(id);
            if (stop == null)
                return NotFound();

            var stopDto = _mapper.Map<RouteStopDto>(stop);
            return Ok(stopDto);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<RouteStopDto>> UpdateStopStatus(int id, [FromBody] UpdateRouteStopStatusDto updateDto)
        {
            var stop = await _routeStopRepository.GetByIdAsync(id);
            if (stop == null)
                return NotFound();

            // Get route for userId
            var route = await _routeRepository.GetByIdAsync(stop.RouteId);
            if (route == null)
                return NotFound("Route not found");

            //If any one Route Stop Status chnges then needs to make main route as the "In Progress"
            if (route.Status == "Not Started")
            {
                route.Status = "In Progress";
                await _routeRepository.UpdateAsync(route);
            }

            // Validate OTP if status is being changed to Delivered/Completed
            if ((updateDto.Status == "Delivered" || updateDto.Status == "Completed") && 
                !string.IsNullOrEmpty(stop.DeliveryOTP))
            {
                if (string.IsNullOrEmpty(updateDto.DeliveryOTP))
                {
                    return BadRequest(new { message = "DeliveryOTP is required to complete this delivery" });
                }
                
                if (updateDto.DeliveryOTP != stop.DeliveryOTP)
                {
                    return BadRequest(new { message = "Invalid DeliveryOTP" });
                }
            }

            stop.Status = updateDto.Status;
            stop.CompletedAt = (updateDto.Status == "Delivered" || updateDto.Status == "Completed") 
                ? DateTime.UtcNow 
                : stop.CompletedAt;

            await _routeStopRepository.UpdateAsync(stop);

            // Update RestockRequest status if associated
            if (stop.RestockRequestId.HasValue && stop.RestockRequestId.Value > 0)
            {
                var restockRequest = await _restockRequestRepository.GetByIdWithDetailsAsync(stop.RestockRequestId.Value);
                if (restockRequest != null)
                {
                    var status = updateDto.Status;
                    restockRequest.Status = MapRouteStatusToRequestStatus(status, "RestockRequest");
                    await _restockRequestRepository.UpdateAsync(restockRequest);

                    //During status completed we were moving this inventory to table , but now doing on outward inventory
                    if (status.ToLower() == "completed")
                    {
                        // Add restock items to LocationInventoryData Table
                        foreach (var item in restockRequest.Items)
                        {
                            // Check if inventory already exists for this location/product/variant
                            var existingInventory = await _locationInventoryDataRepository.GetByLocationAndProductAsync(
                                stop.LocationId,
                                item.ProductId,
                                item.ProductVariantId);

                            if (existingInventory != null)
                            {
                                // Update existing inventory quantity
                                existingInventory.Quantity += item.Quantity;
                                existingInventory.UpdatedBy = route.UserId;
                                existingInventory.UpdatedDate = DateTime.UtcNow;
                                await _locationInventoryDataRepository.UpdateAsync(existingInventory);
                            }
                            else
                            {
                                // Create new inventory record
                                var locationInventory = new NYR.API.Models.Entities.LocationInventoryData
                                {
                                    LocationId = stop.LocationId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    ProductVariantId = item.ProductVariantId,
                                    VariationName = item.ProductVariant?.VariantName,
                                    CreatedBy = route.UserId,
                                    CreatedAt = DateTime.UtcNow
                                };
                                await _locationInventoryDataRepository.AddAsync(locationInventory);
                            }
                        }
                    }
                }
            }

            // Update FollowupRequest status if associated
            if (stop.FollowupRequestId.HasValue && stop.FollowupRequestId.Value > 0)
            {
                var followupRequest = await _followupRequestRepository.GetByIdAsync(stop.FollowupRequestId.Value);
                if (followupRequest != null)
                {
                    var status = updateDto.Status;
                    followupRequest.Status = MapRouteStatusToRequestStatus(status, "FollowupRequest");
                    await _followupRequestRepository.UpdateAsync(followupRequest);
                }
            }

            var stopDto = _mapper.Map<RouteStopDto>(stop);
            return Ok(stopDto);
        }

        [HttpPost("{id}/verify-otp")]
        public async Task<ActionResult> VerifyOTP(int id, [FromBody] VerifyOTPDto verifyDto)
        {
            var stop = await _routeStopRepository.GetByIdAsync(id);
            if (stop == null)
                return NotFound();

            if (string.IsNullOrEmpty(stop.DeliveryOTP))
            {
                return BadRequest(new { message = "No OTP required for this stop" });
            }

            if (verifyDto.OTP != stop.DeliveryOTP)
            {
                return BadRequest(new { message = "Invalid OTP", isValid = false });
            }

            return Ok(new { message = "OTP verified successfully", isValid = true });
        }

        private string MapRouteStatusToRequestStatus(string routeStatus, string requestType)
        {
            return routeStatus switch
            {
                "In Progress" => "In Transit",
                "Completed" => "Delivered",
                "Delivered" => "Delivered",
                "Not Delivered" => requestType == "RestockRequest" ? "Restock Request" : "Followup Requested",
                _ => routeStatus
            };
        }
    }

    public class UpdateRouteStopStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? DeliveryOTP { get; set; }
    }

    public class VerifyOTPDto
    {
        public string OTP { get; set; } = string.Empty;
    }
}
