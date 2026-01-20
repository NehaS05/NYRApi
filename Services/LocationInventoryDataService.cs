using AutoMapper;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NYR.API.Services
{
    public class LocationInventoryDataService : ILocationInventoryDataService
    {
        private readonly ILocationInventoryDataRepository _inventoryRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocationOutwardInventoryService _locationOutwardInventoryService;

        public LocationInventoryDataService(
            ILocationInventoryDataRepository inventoryRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            ApplicationDbContext context,
            IMapper mapper, ILocationOutwardInventoryService locationOutwardInventoryService)
        {
            _inventoryRepository = inventoryRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
            _locationOutwardInventoryService = locationOutwardInventoryService;
        }

        public async Task<IEnumerable<LocationInventoryDataDto>> GetAllInventoryAsync()
        {
            var inventory = await _inventoryRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<LocationInventoryDataDto>>(inventory);
        }

        public async Task<IEnumerable<LocationInventoryGroupDto>> GetAllInventoryGroupedByLocationAsync()
        {
            var groupedInventory = await _inventoryRepository.GetAllGroupedByLocationAsync();
            
            var result = groupedInventory.Select(group => new LocationInventoryGroupDto
            {
                LocationId = group.Key,
                LocationName = group.First().Location?.LocationName ?? "Unknown Location",
                CustomerName = group.First().Location?.Customer?.CompanyName ?? "Unknown Customer",
                //InventoryItems = _mapper.Map<IEnumerable<LocationInventoryDataDto>>(group),
                TotalItems = group.Count(),
                TotalQuantity = group.Sum(item => item.Quantity)
            });

            return result;
        }

        public async Task<LocationInventoryDataDto?> GetInventoryByIdAsync(int id)
        {
            var inventory = await _inventoryRepository.GetByIdWithDetailsAsync(id);
            return inventory != null ? _mapper.Map<LocationInventoryDataDto>(inventory) : null;
        }

        public async Task<IEnumerable<LocationInventoryDataDto>> GetInventoryByLocationIdAsync(int locationId)
        {
            var inventory = await _inventoryRepository.GetByLocationIdAsync(locationId);
            var inventoryDtos = _mapper.Map<IEnumerable<LocationInventoryDataDto>>(inventory);
            
            // Add ProductSKU to each inventory item
            foreach (var dto in inventoryDtos)
            {
                var inventoryItem = inventory.FirstOrDefault(wi => wi.Id == dto.Id);
                dto.ProductSKU = inventoryItem?.ProductVariant?.BarcodeSKU ?? string.Empty;
            }
            
            return inventoryDtos;
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

            // Validate user exists (if CreatedBy is provided)
            if (createDto.CreatedBy.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(createDto.CreatedBy.Value);
                if (user == null)
                    throw new ArgumentException("Invalid user ID");
            }

            // Check if inventory already exists for this location/product/variant
            var existingInventory = await _inventoryRepository.GetByLocationAndProductAsync(
                createDto.LocationId, 
                createDto.ProductId, 
                createDto.ProductVariantId);

            if (existingInventory != null)
            {
                // Update existing inventory quantity
                existingInventory.Quantity += createDto.Quantity;
                existingInventory.UpdatedBy = createDto.CreatedBy;
                existingInventory.UpdatedDate = DateTime.UtcNow;
                await _inventoryRepository.UpdateAsync(existingInventory);
                return await GetInventoryByIdAsync(existingInventory.Id) ?? throw new Exception("Failed to retrieve created inventory");
            }
            else
            {
                // Create new inventory record
                var locationInventory = new NYR.API.Models.Entities.LocationInventoryData
                {
                    LocationId = createDto.LocationId,
                    ProductId = createDto.ProductId,
                    Quantity = createDto.Quantity,
                    ProductVariantId = createDto.ProductVariantId,
                    VariationName = createDto.VariantName,
                    CreatedBy = createDto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };
                var createdInventory = await _inventoryRepository.AddAsync(locationInventory);
                return await GetInventoryByIdAsync(createdInventory.Id) ?? throw new Exception("Failed to retrieve created inventory");
            }            
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

        public async Task<IEnumerable<ProductVariantInfoDto>> GetVariantInfoBySkuAsync(string skuCode, int locationId, int? userId = null, int? productVariantId = null)
        {
            if (string.IsNullOrWhiteSpace(skuCode))
                return Enumerable.Empty<ProductVariantInfoDto>();

            var query = _context.LocationInventoryData
                .Include(lid => lid.ProductVariant)
                .Include(lid => lid.Product)
                .Where(lid => lid.ProductVariant != null && 
                              (lid.ProductVariant.BarcodeSKU == skuCode ||
                               lid.ProductVariant.BarcodeSKU2 == skuCode ||
                               lid.ProductVariant.BarcodeSKU3 == skuCode ||
                               lid.ProductVariant.BarcodeSKU4 == skuCode) 
                               //&& lid.LocationId == locationId
                              && lid.Product.IsActive && lid.ProductVariant.IsActive);

            if (productVariantId.HasValue && productVariantId > 0)
            {
                query = query.Where(pv => pv.ProductVariantId == productVariantId.Value);
            }
            var variantInfo = await query.Select(lid => new ProductVariantInfoDto
                {
                    ProductVariantId = lid.ProductVariant!.Id,
                    VariantName = lid.ProductVariant.VariantName,
                    ProdcutId = lid.ProductId != null ? lid.ProductId : 0
                })
                .Distinct()
                .ToListAsync();


            //If its only one data then do Scan complete 
            if (variantInfo.Count() == 1)
            {
                // Execute CreateOutwardInventoryAsync service for each matching item
                var locationInventoryDatas = await query.ToListAsync();
                foreach (var item in locationInventoryDatas)
                {
                    try
                    {
                        var createOutwardDto = new CreateLocationOutwardInventoryDto
                        {
                            LocationId = locationId,
                            ProductId = item.ProductId,
                            ProductVariantId = item.ProductVariantId,
                            Quantity = 1,  // item.Quantity,  // Quantity is doing one because driver will scan one by one and send
                            VariationName = item.ProductVariant?.VariantName,
                            CreatedBy = userId ?? 1 // Use provided userId or default to 1
                        };

                        await _locationOutwardInventoryService.CreateOutwardInventoryAsync(createOutwardDto);
                        return new List<ProductVariantInfoDto>
                        {
                            new ProductVariantInfoDto
                            {
                                ProductVariantId = item.ProductVariant?.Id ?? 0,
                                VariantName = "Data entered successfully",
                                ProdcutId = item.ProductId
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        return new List<ProductVariantInfoDto>
                        {
                            new ProductVariantInfoDto
                            {
                                ProductVariantId = 0,
                                VariantName = $"Error: {ex.Message}",
                                ProdcutId = 0
                            }
                        };
                    }
                }
            } 
            else if (variantInfo.Count() == 0)
            {
                //When Product is not exist then create one Table for Unlisted Product
                int productIdFromVariant = 0;
                
                // Get Product details from productVariantId if provided
                if (productVariantId.HasValue && productVariantId.Value > 0)
                {
                    var productVariant = await _context.ProductVariants
                        .Include(pv => pv.Product)
                        .FirstOrDefaultAsync(pv => pv.Id == productVariantId.Value);
                    
                    if (productVariant != null && productVariant.Product != null)
                    {
                        productIdFromVariant = productVariant.ProductId;
                        skuCode = productVariant.VariantName;
                    }
                }
                
                var createOutwardDto = new CreateLocationOutwardInventoryDto
                {
                    LocationId = locationId,
                    ProductId = productIdFromVariant, // Use product ID from variant if available, otherwise 0
                    ProductVariantId = productVariantId,
                    Quantity = 1,  // item.Quantity,  // Quantity is doing one because driver will scan one by one and send
                    VariationName = skuCode,        //Barcode
                    CreatedBy = userId ?? 1 // Use provided userId or default to 1
                };
                await _locationOutwardInventoryService.BarcodeOutwardInventoryAsync(createOutwardDto);
            }
            //

            return variantInfo;
        }
    }
}
