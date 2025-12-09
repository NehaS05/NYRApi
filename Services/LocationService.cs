using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ITransferInventoryRepository _transferInventoryRepository;
        private readonly IRestockRequestRepository _restockRequestRepository;

        public LocationService(ILocationRepository locationRepository, ICustomerRepository customerRepository, IUserRepository userRepository, IMapper mapper, ITransferInventoryRepository transferInventoryRepository, IRestockRequestRepository restockRequestRepository)
        {
            _locationRepository = locationRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
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
                }
                catch (Exception)
                {
                    // If there's an error loading transfer items for this location, just skip it
                    locationDto.TransferItems = new List<TransferInventoryItemDto>();
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
    }
}
