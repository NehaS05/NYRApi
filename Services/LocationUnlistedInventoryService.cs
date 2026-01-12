using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class LocationUnlistedInventoryService : ILocationUnlistedInventoryService
    {
        private readonly ILocationUnlistedInventoryRepository _repository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LocationUnlistedInventoryService(
            ILocationUnlistedInventoryRepository repository,
            ILocationRepository locationRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _repository = repository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationUnlistedInventoryDto>> GetAllAsync()
        {
            var inventory = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationUnlistedInventoryDto>>(inventory);
        }

        public async Task<LocationUnlistedInventoryDto?> GetByIdAsync(int id)
        {
            var inventory = await _repository.GetByIdAsync(id);
            return inventory != null ? _mapper.Map<LocationUnlistedInventoryDto>(inventory) : null;
        }

        public async Task<IEnumerable<LocationUnlistedInventoryDto>> GetByLocationIdAsync(int locationId)
        {
            var inventory = await _repository.GetByLocationIdAsync(locationId);
            return _mapper.Map<IEnumerable<LocationUnlistedInventoryDto>>(inventory);
        }

        public async Task<LocationUnlistedInventoryDto?> GetByBarcodeAndLocationAsync(string barcodeNo, int locationId)
        {
            var inventory = await _repository.GetByBarcodeAndLocationAsync(barcodeNo, locationId);
            return inventory != null ? _mapper.Map<LocationUnlistedInventoryDto>(inventory) : null;
        }

        public async Task<LocationUnlistedInventoryDto> CreateAsync(CreateLocationUnlistedInventoryDto createDto)
        {
            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
            if (location == null)
                throw new ArgumentException("Invalid location ID");

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(createDto.CreatedBy);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            // Check if inventory already exists for this barcode and location
            var existingInventory = await _repository.GetByBarcodeAndLocationAsync(createDto.BarcodeNo, createDto.LocationId);
            if (existingInventory != null)
            {
                // Update existing inventory quantity
                existingInventory.Quantity += createDto.Quantity;
                existingInventory.UpdatedBy = createDto.CreatedBy;
                existingInventory.UpdatedDate = DateTime.UtcNow;
                await _repository.UpdateAsync(existingInventory);
                return await GetByIdAsync(existingInventory.Id) ?? throw new Exception("Failed to retrieve updated inventory");
            }

            // Create new inventory record
            var inventory = new LocationUnlistedInventory
            {
                BarcodeNo = createDto.BarcodeNo,
                LocationId = createDto.LocationId,
                Quantity = createDto.Quantity,
                CreatedBy = createDto.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            var createdInventory = await _repository.AddAsync(inventory);
            return await GetByIdAsync(createdInventory.Id) ?? throw new Exception("Failed to retrieve created inventory");
        }

        public async Task<LocationUnlistedInventoryDto?> UpdateAsync(int id, UpdateLocationUnlistedInventoryDto updateDto)
        {
            var inventory = await _repository.GetByIdAsync(id);
            if (inventory == null)
                return null;

            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(updateDto.LocationId);
            if (location == null)
                throw new ArgumentException("Invalid location ID");

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(updateDto.UpdatedBy);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            // Update inventory
            inventory.BarcodeNo = updateDto.BarcodeNo;
            inventory.LocationId = updateDto.LocationId;
            inventory.Quantity = updateDto.Quantity;
            inventory.UpdatedBy = updateDto.UpdatedBy;
            inventory.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(inventory);
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var inventory = await _repository.GetByIdAsync(id);
            if (inventory == null)
                return false;

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            await _repository.DeleteAsync(inventory);
            return true;
        }
    }
}