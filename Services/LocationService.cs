using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationInventoryDataRepository _locationInventoryDataRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITransferInventoryRepository _transferInventoryRepository;
        private readonly IRestockRequestRepository _restockRequestRepository;

        public LocationService(ILocationRepository locationRepository, ICustomerRepository customerRepository, IUserRepository userRepository, ILocationInventoryDataRepository locationInventoryDataRepository, ApplicationDbContext context, IMapper mapper, ITransferInventoryRepository transferInventoryRepository, IRestockRequestRepository restockRequestRepository)
        {
            _locationRepository = locationRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _locationInventoryDataRepository = locationInventoryDataRepository;
            _context = context;
            _mapper = mapper;
            _transferInventoryRepository = transferInventoryRepository;
            _restockRequestRepository = restockRequestRepository;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }
        public async Task<IEnumerable<LocationDto>> GetAllLocationsWithInventoryAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            var locationDtos = _mapper.Map<IEnumerable<LocationDto>>(locations).ToList();
            
            foreach (var locationDto in locationDtos)
            {
                try
                {
                    // Get restock requests for this location
                    var restockRequests = await _restockRequestRepository.GetByLocationIdAsync(locationDto.Id);
                    if (restockRequests != null && restockRequests.Any())
                    {
                        // Flatten all items from all restock requests for this location
                        var allItems = restockRequests.SelectMany(rr => rr.Items).ToList();
                        if (allItems.Any())
                        {
                            // Map RestockRequestItems to TransferInventoryItemDto
                            locationDto.TransferItems = allItems.Select(item => new TransferInventoryItemDto
                            {
                                ProductId = item.ProductId,
                                ProductName = item.Product?.Name ?? string.Empty,
                                SkuCode = item.Product?.BarcodeSKU,
                                ProductVariantId = item.ProductVariantId,
                                VariantName = item.ProductVariant?.VariantName,
                                Quantity = item.Quantity,
                                RestockRequestId = item.RestockRequestId
                            }).ToList();
                        }
                    }

                    // Get location inventory data for this location
                    var locationInventoryData = await _locationInventoryDataRepository.GetByLocationIdAsync(locationDto.Id);
                    if (locationInventoryData != null && locationInventoryData.Any())
                    {
                        locationDto.LocationInventoryData = locationInventoryData.Select(inventory => new LocationInventoryDataDto
                        {
                            Id = inventory.Id,
                            LocationId = inventory.LocationId,
                            LocationName = inventory.Location?.LocationName ?? string.Empty,
                            ProductId = inventory.ProductId,
                            ProductName = inventory.Product?.Name ?? string.Empty,
                            ProductSKU = inventory.Product?.BarcodeSKU ?? string.Empty,
                            ProductVariantId = inventory.ProductVariantId,
                            Quantity = inventory.Quantity,
                            VariantName = inventory.VariationName,
                            CreatedAt = inventory.CreatedAt,
                            CreatedBy = inventory.CreatedBy,
                            CreatedByName = inventory.CreatedByUser?.Name ?? string.Empty,
                            UpdatedBy = inventory.UpdatedBy,
                            UpdatedByName = inventory.UpdatedByUser?.Name,
                            UpdatedDate = inventory.UpdatedDate
                        }).ToList();
                    }
                }
                catch (Exception)
                {
                    // If there's an error loading inventory data for this location, just skip it
                    locationDto.TransferItems = new List<TransferInventoryItemDto>();
                    locationDto.LocationInventoryData = new List<LocationInventoryDataDto>();
                }
            }
            
            return locationDtos;
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int id)
        {
            var location = await _locationRepository.GetLocationWithCustomerAsync(id);
            return location != null ? _mapper.Map<LocationDto>(location) : null;
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(createLocationDto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Invalid customer ID");

            // Validate user exists if UserId is provided
            if (createLocationDto.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(createLocationDto.UserId.Value);
                if (user == null)
                    throw new ArgumentException("Invalid user ID");
            }

            var location = _mapper.Map<Location>(createLocationDto);
            var createdLocation = await _locationRepository.AddAsync(location);
            return _mapper.Map<LocationDto>(createdLocation);
        }

        public async Task<LocationDto?> UpdateLocationAsync(int id, UpdateLocationDto updateLocationDto)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                return null;

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(updateLocationDto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Invalid customer ID");

            // Validate user exists if UserId is provided
            if (updateLocationDto.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(updateLocationDto.UserId.Value);
                if (user == null)
                    throw new ArgumentException("Invalid user ID");
            }

            _mapper.Map(updateLocationDto, location);
            location.UpdatedAt = DateTime.UtcNow;

            await _locationRepository.UpdateAsync(location);
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                return false;

            await _locationRepository.DeleteAsync(location);
            return true;
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsByCustomerAsync(int customerId)
        {
            var locations = await _locationRepository.GetLocationsByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<IEnumerable<LocationDto>> SearchLocationsAsync(string searchTerm)
        {
            var locations = await _locationRepository.SearchLocationsAsync(searchTerm);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsNeedingFollowUpAsync()
        {
            var currentDate = DateTime.UtcNow.Date;
            
            // Get locations with FollowUpDays > 0 and have inventory data
            var locationsNeedingFollowUp = await _context.Locations
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => l.IsActive && l.FollowUpDays.HasValue && l.FollowUpDays > 0)
                .ToListAsync();

            var result = new List<LocationDto>();

            foreach (var location in locationsNeedingFollowUp)
            {
                // Check if location has inventory data
                var hasInventoryData = await _context.LocationInventoryData
                    .AnyAsync(lid => lid.LocationId == location.Id);

                if (!hasInventoryData)
                    continue;

                // Get the last delivery date for this location from Routes
                var lastDeliveryDate = await _context.RouteStops
                    .Where(rs => rs.LocationId == location.Id && rs.IsActive)
                    .Join(_context.Routes, rs => rs.RouteId, r => r.Id, (rs, r) => r.DeliveryDate)
                    .OrderByDescending(deliveryDate => deliveryDate)
                    .FirstOrDefaultAsync();

                // If no delivery found, skip this location
                if (lastDeliveryDate == default(DateTime))
                    continue;

                // Calculate days since last delivery
                var daysSinceLastDelivery = (currentDate - lastDeliveryDate.Date).Days;

                // Check if follow-up is needed (days since last delivery > follow-up days)
                if (daysSinceLastDelivery > location.FollowUpDays.Value)
                {
                    var locationDto = _mapper.Map<LocationDto>(location);
                    
                    // Add additional information about the follow-up
                    locationDto.Comments = $"Last delivery: {lastDeliveryDate:yyyy-MM-dd}, Days since: {daysSinceLastDelivery}, Follow-up needed after: {location.FollowUpDays} days";
                    
                    // Get location inventory data for this location
                    try
                    {
                        var locationInventoryData = await _locationInventoryDataRepository.GetByLocationIdAsync(location.Id);
                        if (locationInventoryData != null && locationInventoryData.Any())
                        {
                            locationDto.LocationInventoryData = locationInventoryData.Select(inventory => new LocationInventoryDataDto
                            {
                                Id = inventory.Id,
                                LocationId = inventory.LocationId,
                                LocationName = inventory.Location?.LocationName ?? string.Empty,
                                ProductId = inventory.ProductId,
                                ProductName = inventory.Product?.Name ?? string.Empty,
                                ProductSKU = inventory.Product?.BarcodeSKU ?? string.Empty,
                                ProductVariantId = inventory.ProductVariantId,
                                Quantity = inventory.Quantity,
                                VariantName = inventory.VariationName,
                                CreatedAt = inventory.CreatedAt,
                                CreatedBy = inventory.CreatedBy,
                                CreatedByName = inventory.CreatedByUser?.Name ?? string.Empty,
                                UpdatedBy = inventory.UpdatedBy,
                                UpdatedByName = inventory.UpdatedByUser?.Name,
                                UpdatedDate = inventory.UpdatedDate
                            }).ToList();
                        }
                    }
                    catch (Exception)
                    {
                        locationDto.LocationInventoryData = new List<LocationInventoryDataDto>();
                    }

                    result.Add(locationDto);
                }
            }

            return result.OrderByDescending(l => l.FollowUpDays);
        }
    }
}
