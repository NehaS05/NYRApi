using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class LocationOutwardInventoryService : ILocationOutwardInventoryService
    {
        private readonly ILocationOutwardInventoryRepository _outwardInventoryRepository;
        private readonly ILocationInventoryDataRepository _locationInventoryDataRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LocationOutwardInventoryService(
            ILocationOutwardInventoryRepository outwardInventoryRepository,
            ILocationInventoryDataRepository locationInventoryDataRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _outwardInventoryRepository = outwardInventoryRepository;
            _locationInventoryDataRepository = locationInventoryDataRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationOutwardInventoryDto>> GetAllOutwardInventoryAsync()
        {
            var inventory = await _outwardInventoryRepository.GetAllWithDetailsAsync();
            var inventoryDtos = _mapper.Map<IEnumerable<LocationOutwardInventoryDto>>(inventory);
            inventoryDtos = inventoryDtos.Where(x => x.IsActive == true);
            // Add ProductSKU to each inventory item
            foreach (var dto in inventoryDtos)
            {
                var inventoryItem = inventory.FirstOrDefault(wi => wi.Id == dto.Id);
                dto.ProductSKU = inventoryItem?.ProductVariant?.BarcodeSKU ?? string.Empty;
            }
            
            return inventoryDtos;
        }

        public async Task<LocationOutwardInventoryDto?> GetOutwardInventoryByIdAsync(int id)
        {
            var inventory = await _outwardInventoryRepository.GetByIdWithDetailsAsync(id);
            if (inventory == null) return null;
            
            var dto = _mapper.Map<LocationOutwardInventoryDto>(inventory);
            dto.ProductSKU = inventory.ProductVariant?.BarcodeSKU ?? string.Empty;
            
            return dto;
        }

        public async Task<IEnumerable<LocationOutwardInventoryDto>> GetOutwardInventoryByLocationIdAsync(int locationId, bool? last60Minutes = null)
        {
            var inventory = await _outwardInventoryRepository.GetByLocationIdAsync(locationId);
            
            // Filter for last 60 minutes if requested
            if (last60Minutes == true)
            {
                var cutoffTime = DateTime.UtcNow.AddMinutes(-60);
                inventory = inventory.Where(i => i.CreatedAt >= cutoffTime);
            }
            
            var inventoryDtos = _mapper.Map<IEnumerable<LocationOutwardInventoryDto>>(inventory);
            
            // Add ProductSKU to each inventory item
            foreach (var dto in inventoryDtos)
            {
                var inventoryItem = inventory.FirstOrDefault(wi => wi.Id == dto.Id);
                dto.ProductSKU = inventoryItem?.ProductVariant?.BarcodeSKU ?? string.Empty;
            }
            
            return inventoryDtos;
        }

        public async Task<IEnumerable<LocationOutwardInventoryDto>> GetOutwardInventoryByProductIdAsync(int productId)
        {
            var inventory = await _outwardInventoryRepository.GetByProductIdAsync(productId);
            var inventoryDtos = _mapper.Map<IEnumerable<LocationOutwardInventoryDto>>(inventory);
            
            // Add ProductSKU to each inventory item
            foreach (var dto in inventoryDtos)
            {
                var inventoryItem = inventory.FirstOrDefault(wi => wi.Id == dto.Id);
                dto.ProductSKU = inventoryItem?.ProductVariant?.BarcodeSKU ?? string.Empty;
            }
            
            return inventoryDtos;
        }

        public async Task<IEnumerable<LocationOutwardInventoryDto>> GetActiveOutwardInventoryByLocationIdAsync(int locationId)
        {
            var inventory = await _outwardInventoryRepository.GetActiveByLocationIdAsync(locationId);
            var inventoryDtos = _mapper.Map<IEnumerable<LocationOutwardInventoryDto>>(inventory);
            
            // Add ProductSKU to each inventory item
            foreach (var dto in inventoryDtos)
            {
                var inventoryItem = inventory.FirstOrDefault(wi => wi.Id == dto.Id);
                dto.ProductSKU = inventoryItem?.ProductVariant?.BarcodeSKU ?? string.Empty;
            }
            
            return inventoryDtos;
        }

        public async Task<LocationOutwardInventoryDto> CreateOutwardInventoryAsync(CreateLocationOutwardInventoryDto createDto)
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

            // Check if corresponding inventory exists in LocationInventoryData
            var existingInventory = await _locationInventoryDataRepository.GetByLocationAndProductAsync(
                createDto.LocationId, 
                createDto.ProductId, 
                createDto.ProductVariantId);

            if (existingInventory == null)
                throw new ArgumentException("No inventory found for this location, product, and variant combination");

            // Check if there's sufficient quantity
            if (existingInventory.Quantity < createDto.Quantity)
                throw new ArgumentException($"Insufficient inventory. Available quantity: {existingInventory.Quantity}, Requested: {createDto.Quantity}");

            // Create outward inventory entry
            var outwardInventory = new LocationOutwardInventory
            {
                LocationId = createDto.LocationId,
                ProductId = createDto.ProductId,
                Quantity = createDto.Quantity,
                ProductVariantId = createDto.ProductVariantId,
                VariationName = createDto.VariationName,
                CreatedBy = createDto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdInventory = await _outwardInventoryRepository.AddAsync(outwardInventory);

            // Decrease quantity in LocationInventoryData
            existingInventory.Quantity -= createDto.Quantity;
            existingInventory.UpdatedBy = createDto.CreatedBy;
            existingInventory.UpdatedDate = DateTime.UtcNow;
            await _locationInventoryDataRepository.UpdateAsync(existingInventory);

            return await GetOutwardInventoryByIdAsync(createdInventory.Id) ?? throw new Exception("Failed to retrieve created outward inventory");
        }

        public async Task<LocationOutwardInventoryDto?> UpdateOutwardInventoryAsync(int id, UpdateLocationOutwardInventoryDto updateDto)
        {
            var inventory = await _outwardInventoryRepository.GetByIdAsync(id);
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

            inventory.LocationId = updateDto.LocationId;
            inventory.ProductId = updateDto.ProductId;
            inventory.Quantity = updateDto.Quantity;
            inventory.ProductVariantId = updateDto.ProductVariantId;
            inventory.VariationName = updateDto.VariationName;
            inventory.UpdatedBy = updateDto.UpdatedBy;
            inventory.UpdatedDate = DateTime.UtcNow;
            inventory.IsActive = updateDto.IsActive;

            await _outwardInventoryRepository.UpdateAsync(inventory);
            return await GetOutwardInventoryByIdAsync(id);
        }

        public async Task<bool> DeleteOutwardInventoryAsync(int id, int userId)
        {
            var outwardInventory = await _outwardInventoryRepository.GetByIdAsync(id);
            if (outwardInventory == null)
                return false;

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            // Only restore inventory if the record is currently active
            if (outwardInventory.IsActive)
            {
                // Find the corresponding inventory in LocationInventoryData
                var locationInventory = await _locationInventoryDataRepository.GetByLocationAndProductAsync(
                    outwardInventory.LocationId, 
                    outwardInventory.ProductId, 
                    outwardInventory.ProductVariantId);

                if (locationInventory != null)
                {
                    // Restore the quantity back to LocationInventoryData
                    locationInventory.Quantity += outwardInventory.Quantity;
                    locationInventory.UpdatedBy = userId;
                    locationInventory.UpdatedDate = DateTime.UtcNow;
                    await _locationInventoryDataRepository.UpdateAsync(locationInventory);
                }
            }

            // Soft delete: Set IsActive = false
            outwardInventory.IsActive = false;
            outwardInventory.UpdatedBy = userId;
            outwardInventory.UpdatedDate = DateTime.UtcNow;
            await _outwardInventoryRepository.UpdateAsync(outwardInventory);

            return true;
        }

        public async Task<bool> DeactivateOutwardInventoryAsync(int id, int userId)
        {
            var inventory = await _outwardInventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return false;

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Invalid user ID");

            inventory.IsActive = false;
            inventory.UpdatedBy = userId;
            inventory.UpdatedDate = DateTime.UtcNow;

            await _outwardInventoryRepository.UpdateAsync(inventory);
            return true;
        }
    }
}