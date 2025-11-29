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
        private readonly ITransferInventoryRepository _transferInventoryRepository;
        private readonly IMapper _mapper;

        public RouteService(
            IRouteRepository routeRepository,
            IRouteStopRepository routeStopRepository,
            ILocationRepository locationRepository,
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            ITransferInventoryRepository transferInventoryRepository,
            IMapper mapper)
        {
            _routeRepository = routeRepository;
            _routeStopRepository = routeStopRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _transferInventoryRepository = transferInventoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllWithDetailsAsync();
            var routeDtos = _mapper.Map<IEnumerable<RouteDto>>(routes).ToList();
            
            // Load shipping inventory for each route stop
            foreach (var routeDto in routeDtos)
            {
                await LoadShippingInventoryForRouteStops(routeDto);
            }
            
            return routeDtos;
        }

        public async Task<RouteDto?> GetRouteByIdAsync(int id)
        {
            var route = await _routeRepository.GetByIdWithDetailsAsync(id);
            if (route == null) return null;
            
            var routeDto = _mapper.Map<RouteDto>(route);
            await LoadShippingInventoryForRouteStops(routeDto);
            
            return routeDto;
        }
        
        private async Task LoadShippingInventoryForRouteStops(RouteDto routeDto)
        {
            foreach (var stop in routeDto.RouteStops)
            {
                try
                {
                    var transfers = await _transferInventoryRepository.GetByLocationIdAsync(stop.LocationId);
                    if (transfers != null && transfers.Any())
                    {
                        var allItems = transfers.SelectMany(t => t.Items).ToList();
                        if (allItems.Any())
                        {
                            stop.ShippingInventory = _mapper.Map<List<TransferInventoryItemDto>>(allItems);
                        }
                    }
                }
                catch (Exception)
                {
                    // If there's an error loading shipping inventory, just skip it
                    stop.ShippingInventory = new List<TransferInventoryItemDto>();
                }
            }
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(createRouteDto.UserId);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            // Validate locations and customers exist for all stops
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
            }

            var route = _mapper.Map<Routes>(createRouteDto);
            var createdRoute = await _routeRepository.AddAsync(route);

            // Create route stops
            foreach (var stopDto in createRouteDto.RouteStops)
            {
                var stop = _mapper.Map<RouteStop>(stopDto);
                stop.RouteId = createdRoute.Id;
                await _routeStopRepository.AddAsync(stop);
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

            // Validate locations and customers exist for all stops
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
            }

            _mapper.Map(updateRouteDto, route);
            route.UpdatedAt = DateTime.UtcNow;
            await _routeRepository.UpdateAsync(route);

            // Update route stops
            var existingStops = await _routeStopRepository.GetByRouteIdAsync(id);

            // Remove stops not in the update
            var stopsToRemove = existingStops.Where(es => !updateRouteDto.RouteStops.Any(s => s.Id == es.Id)).ToList();
            foreach (var stop in stopsToRemove)
            {
                await _routeStopRepository.DeleteAsync(stop.Id);
            }

            // Add or update stops
            foreach (var stopDto in updateRouteDto.RouteStops)
            {
                if (stopDto.Id.HasValue)
                {
                    var existingStop = existingStops.FirstOrDefault(es => es.Id == stopDto.Id.Value);
                    if (existingStop != null)
                    {
                        _mapper.Map(stopDto, existingStop);
                        await _routeStopRepository.UpdateAsync(existingStop);
                    }
                }
                else
                {
                    var newStop = _mapper.Map<RouteStop>(stopDto);
                    newStop.RouteId = id;
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
            var routeDtos = _mapper.Map<IEnumerable<RouteDto>>(routes).ToList();
            
            foreach (var routeDto in routeDtos)
            {
                await LoadShippingInventoryForRouteStops(routeDto);
            }
            
            return routeDtos;
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
    }
}
