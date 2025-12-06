using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class LocationInventoryDataService : ILocationInventoryDataService
    {
        private readonly ILocationInventoryDataRepository _inventoryRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LocationInventoryDataService(
            ILocationInventoryDataRepository inventoryRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationInventoryDataDto>> GetAllInventoryAsync()
        {
            var inventory = await _inventoryRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<LocationInventoryDataDto>>(inventory);
        }

        public async Task<LocationInventoryDataDto?> GetInventoryByIdAsync(int id)
        {
            var inventory = await _inventoryRepository.GetByIdWithDetailsAsync(id);
            return inventory != null ? _mapper.Map<LocationInventoryDataDto>(inventory) : null;
        }

        public async Task<IEnumerable<LocationInventoryDataDto>> GetInventoryByLocationIdAsync(int locationId)
        {
            var inventory = await _inventoryRepository.GetByLocationIdAsync(locationId);
            return _mapper.Map<IEnumerable<LocationInventoryDataDto>>(inventory);
        }

        public async Task<IEnumerable<LocationInventoryDataDto>> GetInventoryByProductIdAsync(int productId)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<LocationInventoryDataDto>>(inventory);
        }

        public async Task<LocationInventoryDataDto> CreateInventoryAsync(CreateLocationInventoryDataDto createDto)
        {
            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
            if (location == null)
                throw new ArgumentException("Invalid location ID");

            // Validate product exists
            var product = await _productRepository.GetByIdAsync(createDto.ProductId);
            if (product == null)
                throw new ArgumentException("Invalid product ID");

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(createDto.CreatedBy);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            // Check if inventory already exists for this combination
            var existing = await _inventoryRepository.GetByLocationAndProductVariationNameAsync(
                createDto.LocationId, 
                createDto.ProductId, 
                createDto.VariationName);

            if (existing != null)
                throw new ArgumentException("Inventory already exists for this location, product, and variation combination");

            var inventory = _mapper.Map<LocationInventoryData>(createDto);
            var createdInventory = await _inventoryRepository.AddAsync(inventory);

            return await GetInventoryByIdAsync(createdInventory.Id) ?? throw new Exception("Failed to retrieve created inventory");
        }

        public async Task<LocationInventoryDataDto?> UpdateInventoryAsync(int id, UpdateLocationInventoryDataDto updateDto)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return null;

            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(updateDto.LocationId);
            if (location == null)
                throw new ArgumentException("Invalid location ID");

            // Validate product exists
            var product = await _productRepository.GetByIdAsync(updateDto.ProductId);
            if (product == null)
                throw new ArgumentException("Invalid product ID");

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(updateDto.UpdatedBy);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            _mapper.Map(updateDto, inventory);
            inventory.UpdatedDate = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventory);
            return await GetInventoryByIdAsync(id);
        }

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return false;

            await _inventoryRepository.DeleteAsync(inventory);
            return true;
        }

        public async Task<LocationInventoryDataDto?> AdjustQuantityAsync(int id, int quantityChange, int userId)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return null;

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            inventory.Quantity += quantityChange;
            
            // Ensure quantity doesn't go negative
            if (inventory.Quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            inventory.UpdatedBy = userId;
            inventory.UpdatedDate = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventory);
            return await GetInventoryByIdAsync(id);
        }
    }
}
