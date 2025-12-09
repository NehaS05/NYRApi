using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IRouteStopRepository _routeStopRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRestockRequestRepository _restockRequestRepository;
        private readonly IFollowupRequestRepository _followupRequestRepository;
        private readonly ILocationInventoryDataRepository _locationInventoryDataRepository;
        private readonly IMapper _mapper;

        public RouteService(
            IRouteRepository routeRepository,
            IRouteStopRepository routeStopRepository,
            ILocationRepository locationRepository,
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IRestockRequestRepository restockRequestRepository,
            IFollowupRequestRepository followupRequestRepository,
            ILocationInventoryDataRepository locationInventoryDataRepository,
            IMapper mapper)
        {
            _routeRepository = routeRepository;
            _routeStopRepository = routeStopRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _restockRequestRepository = restockRequestRepository;
            _followupRequestRepository = followupRequestRepository;
            _locationInventoryDataRepository = locationInventoryDataRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllWithDetailsAsync();
            var routeDtos = _mapper.Map<IEnumerable<RouteDto>>(routes).ToList();
            routeDtos = routeDtos.Where(r => r.IsActive).ToList();

            // Cache LocationInventoryData by locationId to avoid redundant DB calls
            var locationInventoryCache = new Dictionary<int, List<LocationInventoryDataDto>>();

            // Get LocationInventoryData for each route stop
            foreach (var routeDto in routeDtos)
            {
                foreach (var stopDto in routeDto.RouteStops)
                {
                    // Check if we already fetched data for this location
                    if (!locationInventoryCache.ContainsKey(stopDto.LocationId))
                    {
                        var locationInventoryData = await _locationInventoryDataRepository.GetByLocationIdAsync(stopDto.LocationId);
                        var mappedData = locationInventoryData != null && locationInventoryData.Any()
                            ? _mapper.Map<List<LocationInventoryDataDto>>(locationInventoryData)
                            : new List<LocationInventoryDataDto>();
                        
                        locationInventoryCache[stopDto.LocationId] = mappedData;
                    }

                    // Use cached data
                    stopDto.LocationInventory = locationInventoryCache[stopDto.LocationId];
                }
            }
           
            return routeDtos;
        }

        public async Task<RouteDto?> GetRouteByIdAsync(int id)
        {
            var route = await _routeRepository.GetByIdWithDetailsAsync(id);
            if (route == null) return null;
            
            var routeDto = _mapper.Map<RouteDto>(route);
            
            return routeDto;
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(createRouteDto.UserId);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            // Validate locations, customers, restock requests, and followup requests exist for all stops
            foreach (var stopDto in createRouteDto.RouteStops)
            {
                var location = await _locationRepository.GetByIdAsync(stopDto.LocationId);
                if (location == null)
                    throw new ArgumentException($"Invalid location ID: {stopDto.LocationId}");

                if (stopDto.CustomerId.HasValue)
                {
                    var customer = await _customerRepository.GetByIdAsync(stopDto.CustomerId.Value);
                    if (customer == null)
                        throw new ArgumentException($"Invalid customer ID: {stopDto.CustomerId}");
                }

                if (stopDto.RestockRequestId.HasValue && stopDto.RestockRequestId.Value > 0)
                {
                    var restockRequest = await _restockRequestRepository.GetByIdAsync(stopDto.RestockRequestId.Value);
                    if (restockRequest == null)
                        throw new ArgumentException($"Invalid restock request ID: {stopDto.RestockRequestId}");
                }
                else if (stopDto.RestockRequestId.HasValue && stopDto.RestockRequestId.Value == 0)
                {
                    // Convert 0 to null
                    stopDto.RestockRequestId = null;
                }

                if (stopDto.FollowupRequestId.HasValue && stopDto.FollowupRequestId.Value > 0)
                {
                    var followupRequest = await _followupRequestRepository.GetByIdAsync(stopDto.FollowupRequestId.Value);
                    if (followupRequest == null)
                        throw new ArgumentException($"Invalid followup request ID: {stopDto.FollowupRequestId}");
                }
                else if (stopDto.FollowupRequestId.HasValue && stopDto.FollowupRequestId.Value == 0)
                {
                    // Convert 0 to null
                    stopDto.FollowupRequestId = null;
                }
            }

            var route = _mapper.Map<Routes>(createRouteDto);
            var createdRoute = await _routeRepository.AddAsync(route);

            // Create route stops and update associated request statuses
            foreach (var stopDto in createRouteDto.RouteStops)
            {
                var stop = _mapper.Map<RouteStop>(stopDto);
                stop.RouteId = createdRoute.Id;
                stop.DeliveryOTP = GenerateOTP();
                await _routeStopRepository.AddAsync(stop);

                // Update RestockRequest status to "Draft" if associated
                if (stopDto.RestockRequestId.HasValue && stopDto.RestockRequestId.Value > 0)
                {
                    var restockRequest = await _restockRequestRepository.GetByIdAsync(stopDto.RestockRequestId.Value);
                    if (restockRequest != null)
                    {
                        restockRequest.Status = createRouteDto.Status == "In Progress" ? "In Transit" : restockRequest.Status;
                        await _restockRequestRepository.UpdateAsync(restockRequest);
                    }
                }

                // Update FollowupRequest status to "Draft" if associated
                if (stopDto.FollowupRequestId.HasValue && stopDto.FollowupRequestId.Value > 0)
                {
                    var followupRequest = await _followupRequestRepository.GetByIdAsync(stopDto.FollowupRequestId.Value);
                    if (followupRequest != null)
                    {
                        followupRequest.Status = createRouteDto.Status == "In Progress" ? "In Transit" : followupRequest.Status;
                        await _followupRequestRepository.UpdateAsync(followupRequest);
                    }
                }
            }

            return await GetRouteByIdAsync(createdRoute.Id) ?? throw new Exception("Failed to retrieve created route");
        }

        public async Task<RouteDto?> UpdateRouteAsync(int id, UpdateRouteDto updateRouteDto)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null)
                return null;

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(updateRouteDto.UserId);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            // Validate locations, customers, restock requests, and followup requests exist for all stops
            foreach (var stopDto in updateRouteDto.RouteStops)
            {
                var location = await _locationRepository.GetByIdAsync(stopDto.LocationId);
                if (location == null)
                    throw new ArgumentException($"Invalid location ID: {stopDto.LocationId}");

                if (stopDto.CustomerId.HasValue)
                {
                    var customer = await _customerRepository.GetByIdAsync(stopDto.CustomerId.Value);
                    if (customer == null)
                        throw new ArgumentException($"Invalid customer ID: {stopDto.CustomerId}");
                }

                if (stopDto.RestockRequestId.HasValue && stopDto.RestockRequestId.Value > 0)
                {
                    var restockRequest = await _restockRequestRepository.GetByIdAsync(stopDto.RestockRequestId.Value);
                    if (restockRequest == null)
                        throw new ArgumentException($"Invalid restock request ID: {stopDto.RestockRequestId}");
                }
                else if (stopDto.RestockRequestId.HasValue && stopDto.RestockRequestId.Value == 0)
                {
                    // Convert 0 to null
                    stopDto.RestockRequestId = null;
                }

                if (stopDto.FollowupRequestId.HasValue && stopDto.FollowupRequestId.Value > 0)
                {
                    var followupRequest = await _followupRequestRepository.GetByIdAsync(stopDto.FollowupRequestId.Value);
                    if (followupRequest == null)
                        throw new ArgumentException($"Invalid followup request ID: {stopDto.FollowupRequestId}");
                }
                else if (stopDto.FollowupRequestId.HasValue && stopDto.FollowupRequestId.Value == 0)
                {
                    // Convert 0 to null
                    stopDto.FollowupRequestId = null;
                }
            }

            //Check any of the Route Stop is changed to Completed or Not Delivered then Make main Route to "In Progress"
            var mainRouteStatus = updateRouteDto.RouteStops.Where(x => x.Status == "In Progress").ToList().Count > 0
                ? "In Progress" : updateRouteDto.Status;
            updateRouteDto.Status = mainRouteStatus;

            _mapper.Map(updateRouteDto, route);
            route.UpdatedAt = DateTime.UtcNow;
            await _routeRepository.UpdateAsync(route);

            // Update route stops
            var existingStops = await _routeStopRepository.GetByRouteIdAsync(id);

            // Remove stops not in the update  //No need to delete for now
            var stopsToRemove = existingStops.Where(es => !updateRouteDto.RouteStops.Any(s => s.Id == es.Id)).ToList();
            foreach (var stop in stopsToRemove)
            {
                await _routeStopRepository.DeleteAsync(stop.Id);
            }

            // Add or update stops and update associated request statuses
            foreach (var stopDto in updateRouteDto.RouteStops)
            {                
                if (stopDto.Id.HasValue && stopDto.Id.Value > 0)
                {
                    stopDto.Status = "Pending";
                    var existingStop = existingStops.FirstOrDefault(es => es.Id == stopDto.Id.Value);
                    if (existingStop != null)
                    {
                        stopDto.DeliveryOTP = existingStop.DeliveryOTP;
                        // Map and update the stop
                        _mapper.Map(stopDto, existingStop);
                        await _routeStopRepository.UpdateAsync(existingStop);
                    }
                }
                else
                {
                    stopDto.Status = "Pending";
                    var newStop = _mapper.Map<RouteStop>(stopDto);
                    newStop.RouteId = id;
                    newStop.DeliveryOTP = GenerateOTP();
                    await _routeStopRepository.AddAsync(newStop);
                }                
            }

            return await GetRouteByIdAsync(id);
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            return await _routeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RouteDto>> GetRoutesByLocationIdAsync(int locationId)
        {
            var routes = await _routeRepository.GetByLocationIdAsync(locationId);
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<IEnumerable<RouteDto>> GetRoutesByUserIdAsync(int userId)
        {
            var routes = await _routeRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<IEnumerable<RouteDto>> GetRoutesByDeliveryDateAsync(DateTime deliveryDate)
        {
            var routes = await _routeRepository.GetByDeliveryDateAsync(deliveryDate);
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<RouteDto?> UpdateRouteStatusAsync(int id, string status)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null)
                return null;

            route.Status = status;
            route.UpdatedAt = DateTime.UtcNow;
            await _routeRepository.UpdateAsync(route);

            return await GetRouteByIdAsync(id);
        }

        public async Task<IEnumerable<RouteDto>> GetRoutesByStatusAsync(string status)
        {
            var routes = await _routeRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        /// <summary>
        /// Maps route status to request status
        /// </summary>
        private string MapRouteStatusToRequestStatus(string routeStatus, string requestType)
        {
            //When Route Status is In Progress then make RestockRequest/FollowupRequest status as In Transit
            //When Route Status is Completed then Delivered
            //When Route Status is Not Delivered then Restock Request/Followup requested
            return routeStatus switch
            {
                "In Progress" => "In Transit",
                "Completed" => "Delivered",
                "Not Delivered" => requestType == "RestockRequest" ? "Restock Request" : "Followup Requested",
                _ => routeStatus
            };
        }

        /// <summary>
        /// Generates a random 6-digit OTP
        /// </summary>
        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
