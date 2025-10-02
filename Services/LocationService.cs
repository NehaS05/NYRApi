using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, ICustomerRepository customerRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
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
