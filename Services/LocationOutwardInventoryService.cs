using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        private readonly ILocationUnlistedInventoryRepository _locationUnlistedInventoryRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<ProductVariant> _productVariantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LocationOutwardInventoryService(
            ILocationOutwardInventoryRepository outwardInventoryRepository,
            ILocationInventoryDataRepository locationInventoryDataRepository,
            ILocationUnlistedInventoryRepository locationUnlistedInventoryRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IGenericRepository<ProductVariant> productVariantRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _outwardInventoryRepository = outwardInventoryRepository;
            _locationInventoryDataRepository = locationInventoryDataRepository;
            _locationUnlistedInventoryRepository = locationUnlistedInventoryRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
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
            
            // Get data from LocationUnlistedInventories as well by location Id
            var unlistedInventory = await _locationUnlistedInventoryRepository.GetByLocationIdAsync(locationId);
            
            // Filter for last 60 minutes if requested
            if (last60Minutes == true)
            {
                var cutoffTime = DateTime.UtcNow.AddMinutes(-60);
                inventory = inventory.Where(i => i.CreatedAt >= cutoffTime);
                unlistedInventory = unlistedInventory.Where(i => i.CreatedDate >= cutoffTime);
            }
            
            var inventoryDtos = _mapper.Map<IEnumerable<LocationOutwardInventoryDto>>(inventory);
            
            // Add ProductSKU to each inventory item
            foreach (var dto in inventoryDtos)
            {
                var inventoryItem = inventory.FirstOrDefault(wi => wi.Id == dto.Id);
                dto.ProductSKU = inventoryItem?.ProductVariant?.BarcodeSKU ?? string.Empty;
            }
            
            // Convert unlisted inventory to LocationOutwardInventoryDto format
            var unlistedInventoryDtos = unlistedInventory.Select(ui => new LocationOutwardInventoryDto
            {
                Id = ui.Id,
                LocationId = ui.LocationId,
                LocationName = ui.Location?.LocationName ?? string.Empty,
                ProductId = 0, // No specific product ID for unlisted items
                ProductName = "Unlisted Item",
                ProductSKU = ui.BarcodeNo,
                Quantity = ui.Quantity,
                CreatedAt = ui.CreatedDate,
                CreatedBy = ui.CreatedBy,
                CreatedByName = ui.CreatedByUser?.Name ?? "System",
                UpdatedBy = ui.UpdatedBy,
                UpdatedByName = ui.UpdatedByUser?.Name,
                UpdatedDate = ui.UpdatedDate,
                ProductVariantId = null,
                VariationName = "Unlisted",
                IsActive = true
            });
            
            // Combine both datasets
            var combinedResult = inventoryDtos.Concat(unlistedInventoryDtos).OrderByDescending(x => x.CreatedAt);
            
            return combinedResult;
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
            //if (existingInventory.Quantity < createDto.Quantity)
            //    throw new ArgumentException($"Insufficient inventory. Available quantity: {existingInventory.Quantity}, Requested: {createDto.Quantity}");

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

        public async Task<LocationOutwardInventoryDto> BarcodeOutwardInventoryAsync(CreateLocationOutwardInventoryDto createDto)
        {
            try
            {
                // Validate location exists
                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
                if (location == null)
                    throw new ArgumentException("Invalid location ID");

                // Validate product exists (only if ProductId > 0)
                Product? product = null;
                if (createDto.ProductId > 0)
                {
                    product = await _productRepository.GetByIdAsync(createDto.ProductId);
                    if (product == null)
                        throw new ArgumentException("Invalid product ID");
                }

                // Validate user exists
                var user = await _userRepository.GetByIdAsync(createDto.CreatedBy);
                if (user == null)
                    throw new ArgumentException("Invalid user ID");

                // Check if corresponding inventory exists in LocationInventoryData (only for valid products)
                LocationInventoryData? existingInventory = null;
                if (createDto.ProductId > 0)
                {
                    existingInventory = await _locationInventoryDataRepository.GetByLocationAndProductAsync(
                        createDto.LocationId,
                        createDto.ProductId,
                        createDto.ProductVariantId);
                }

                var variationName = "";

                //When Product is not exist then create one Table for Unlisted Product
                if (existingInventory == null && createDto.ProductId > 0)
                {
                    // Additional check: Verify if ProductId and ProductVariantId combination exists in ProductVariants table
                    bool productVariantExists = true;
                    if (createDto.ProductVariantId.HasValue)
                    {
                        var productVariant = await _productVariantRepository.GetByIdAsync(createDto.ProductVariantId.Value);
                        // productVariantExists = productVariant != null && productVariant.ProductId == createDto.ProductId;
                        productVariantExists = productVariant != null;
                        variationName = productVariant != null ? productVariant.VariantName : "";
                    }

                    // Only create unlisted inventory if both conditions are met:
                    // 1. No inventory exists in LocationInventoryData
                    // 2. ProductVariantId combination doesn't exist in ProductVariants table (if ProductVariantId is provided)
                    if (!productVariantExists || createDto.ProductVariantId == null)
                    {
                        // Get the barcode from the product
                        var barcodeNo = createDto.VariationName != null ? createDto.VariationName : "";

                        // Check if this barcode already exists in unlisted inventory for this location
                        var existingUnlistedInventory = await _locationUnlistedInventoryRepository
                            .GetByBarcodeAndLocationAsync(barcodeNo, createDto.LocationId);

                        //if (existingUnlistedInventory != null)
                        //{
                        //    // Update existing unlisted inventory quantity
                        //    existingUnlistedInventory.Quantity += createDto.Quantity;
                        //    existingUnlistedInventory.UpdatedBy = createDto.CreatedBy;
                        //    existingUnlistedInventory.UpdatedDate = DateTime.UtcNow;
                        //    await _locationUnlistedInventoryRepository.UpdateAsync(existingUnlistedInventory);
                        //}
                        //else
                        //{
                            // Create new unlisted inventory entry
                            var unlistedInventory = new LocationUnlistedInventory
                            {
                                BarcodeNo = barcodeNo,
                                LocationId = createDto.LocationId,
                                Quantity = createDto.Quantity,
                                CreatedBy = createDto.CreatedBy,
                                CreatedDate = DateTime.UtcNow
                            };

                            await _locationUnlistedInventoryRepository.AddAsync(unlistedInventory);
                        //}

                        // Still throw exception to maintain existing API behavior, but unlisted inventory is now recorded
                        throw new ArgumentException("No inventory found for this location, product, and variant combination. Item has been recorded as unlisted inventory.");
                    }
                    else
                    {
                        // ProductVariant exists but no inventory - this is a different scenario
                        //throw new ArgumentException("No inventory found for this location, product, and variant combination");
                    }
                }
                else if (createDto.ProductId == 0)
                {
                    // Handle unlisted product case (ProductId = 0)
                    var barcodeNo = createDto.VariationName ?? "";

                    // Check if this barcode already exists in unlisted inventory for this location
                    var existingUnlistedInventory = await _locationUnlistedInventoryRepository
                        .GetByBarcodeAndLocationAsync(barcodeNo, createDto.LocationId);

                    //if (existingUnlistedInventory != null)
                    //{
                    //    // Update existing unlisted inventory quantity
                    //    existingUnlistedInventory.Quantity += createDto.Quantity;
                    //    existingUnlistedInventory.UpdatedBy = createDto.CreatedBy;
                    //    existingUnlistedInventory.UpdatedDate = DateTime.UtcNow;
                    //    await _locationUnlistedInventoryRepository.UpdateAsync(existingUnlistedInventory);
                    //}
                    //else
                    //{
                        // Create new unlisted inventory entry
                        var unlistedInventory = new LocationUnlistedInventory
                        {
                            BarcodeNo = barcodeNo,
                            LocationId = createDto.LocationId,
                            Quantity = createDto.Quantity,
                            CreatedBy = createDto.CreatedBy,
                            CreatedDate = DateTime.UtcNow
                        };

                        await _locationUnlistedInventoryRepository.AddAsync(unlistedInventory);
                    //}
                }

                // Check if there's sufficient quantity 
                //if (existingInventory.Quantity < createDto.Quantity)
                //    throw new ArgumentException($"Insufficient inventory. Available quantity: {existingInventory.Quantity}, Requested: {createDto.Quantity}");

                // Create outward inventory entry
                var createdInventoryId = 0;
                if (createDto.ProductId > 0)
                {
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
                    createdInventoryId = createdInventory.Id;
                }

                // Decrease quantity in LocationInventoryData (only for valid products)
                if (existingInventory != null)
                {
                    existingInventory.Quantity -= createDto.Quantity;
                    existingInventory.UpdatedBy = createDto.CreatedBy;
                    existingInventory.UpdatedDate = DateTime.UtcNow;
                    await _locationInventoryDataRepository.UpdateAsync(existingInventory);
                }
                else if (createDto.ProductId > 0)
                {
                    //Or Add Quantity with Minus data in LocationInventoryData
                    // Create new inventory record
                    var locationInventory = new NYR.API.Models.Entities.LocationInventoryData
                    {
                        LocationId = createDto.LocationId,
                        ProductId = createDto.ProductId,
                        Quantity = 0 - createDto.Quantity,
                        ProductVariantId = createDto.ProductVariantId,
                        VariationName = variationName,
                        CreatedBy = 1,  //It's location based entry default by 1 id
                        CreatedAt = DateTime.UtcNow
                    };
                    await _locationInventoryDataRepository.AddAsync(locationInventory);
                }

                return await GetOutwardInventoryByIdAsync(createdInventoryId);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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