using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class WarehouseInventoryService : IWarehouseInventoryService
    {
        private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
        private readonly IGenericRepository<Warehouse> _warehouseRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductVariant> _productVariantRepository;
        private readonly IMapper _mapper;

        public WarehouseInventoryService(
            IWarehouseInventoryRepository warehouseInventoryRepository,
            IGenericRepository<Warehouse> warehouseRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductVariant> productVariantRepository,
            IMapper mapper)
        {
            _warehouseInventoryRepository = warehouseInventoryRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _mapper = mapper;
        }

        public async Task<WarehouseInventoryDto> AddInventoryAsync(AddInventoryDto addInventoryDto)
        {
            // Validate warehouse exists
            var warehouse = await _warehouseRepository.GetByIdAsync(addInventoryDto.WarehouseId);
            if (warehouse == null)
                throw new ArgumentException("Warehouse not found");

            // Validate product exists
            var product = await _productRepository.GetByIdAsync(addInventoryDto.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found");

            // Validate product variant exists (if specified)
            if (addInventoryDto.ProductVariantId.HasValue)
            {
                var productVariant = await _productVariantRepository.GetByIdAsync(addInventoryDto.ProductVariantId.Value);
                if (productVariant == null)
                    throw new ArgumentException("Product variant not found");
            }

            // Check if inventory already exists for this warehouse and product variant
            WarehouseInventory? existingInventory = null;
            if (addInventoryDto.ProductVariantId.HasValue)
            {
                existingInventory = await _warehouseInventoryRepository
                    .GetByWarehouseAndProductVariantAsync(addInventoryDto.WarehouseId, addInventoryDto.ProductVariantId.Value);
            }

            if (existingInventory != null)
            {
                // Update existing inventory quantity
                existingInventory.Quantity += addInventoryDto.Quantity;
                existingInventory.UpdatedAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(addInventoryDto.Notes))
                {
                    existingInventory.Notes = addInventoryDto.Notes;
                }

                await _warehouseInventoryRepository.UpdateAsync(existingInventory);
                return await GetInventoryByIdAsync(existingInventory.Id) ?? 
                       throw new InvalidOperationException("Failed to retrieve updated inventory");
            }
            else
            {
                // Create new inventory entry
                var inventory = new WarehouseInventory
                {
                    WarehouseId = addInventoryDto.WarehouseId,
                    ProductId = addInventoryDto.ProductId,
                    ProductVariantId = addInventoryDto.ProductVariantId,
                    Quantity = addInventoryDto.Quantity,
                    Notes = addInventoryDto.Notes
                };

                var createdInventory = await _warehouseInventoryRepository.AddAsync(inventory);
                return await GetInventoryByIdAsync(createdInventory.Id) ?? 
                       throw new InvalidOperationException("Failed to retrieve created inventory");
            }
        }

        public async Task<IEnumerable<WarehouseInventoryDto>> AddBulkInventoryAsync(AddBulkInventoryDto addBulkInventoryDto)
        {
            // Validate warehouse exists
            var warehouse = await _warehouseRepository.GetByIdAsync(addBulkInventoryDto.WarehouseId);
            if (warehouse == null)
                throw new ArgumentException("Warehouse not found");

            // Validate product exists
            var product = await _productRepository.GetByIdAsync(addBulkInventoryDto.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var results = new List<WarehouseInventoryDto>();

            foreach (var item in addBulkInventoryDto.InventoryItems)
            {
                // Validate product variant exists
                if (item.ProductVariantId.HasValue)
                {
                    var productVariant = await _productVariantRepository.GetByIdAsync(item.ProductVariantId.Value);
                    if (productVariant == null)
                        throw new ArgumentException($"Product variant with ID {item.ProductVariantId} not found");
                }

                // Check if inventory already exists for this warehouse and product variant
                WarehouseInventory? existingInventory = null;
                if (item.ProductVariantId.HasValue)
                {
                    existingInventory = await _warehouseInventoryRepository
                        .GetByWarehouseAndProductVariantAsync(addBulkInventoryDto.WarehouseId, item.ProductVariantId.Value);
                }

                if (existingInventory != null)
                {
                    // Update existing inventory quantity
                    existingInventory.Quantity += item.Quantity;
                    existingInventory.UpdatedAt = DateTime.UtcNow;
                    if (!string.IsNullOrEmpty(item.Notes))
                    {
                        existingInventory.Notes = item.Notes;
                    }

                    await _warehouseInventoryRepository.UpdateAsync(existingInventory);
                    var updatedInventoryDto = await GetInventoryByIdAsync(existingInventory.Id);
                    if (updatedInventoryDto != null)
                    {
                        results.Add(updatedInventoryDto);
                    }
                }
                else
                {
                    // Create new inventory entry
                    var newInventory = new WarehouseInventory
                    {
                        WarehouseId = addBulkInventoryDto.WarehouseId,
                        ProductId = addBulkInventoryDto.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        Quantity = item.Quantity,
                        Notes = item.Notes
                    };

                    var createdInventory = await _warehouseInventoryRepository.AddAsync(newInventory);
                    var createdInventoryDto = await GetInventoryByIdAsync(createdInventory.Id);
                    if (createdInventoryDto != null)
                    {
                        results.Add(createdInventoryDto);
                    }
                }
            }

            return results;
        }

        public async Task<IEnumerable<WarehouseListDto>> GetWarehouseListAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            warehouses = warehouses.Where(x => x.IsActive == true);
            var productCounts = await _warehouseInventoryRepository.GetWarehouseProductCountsAsync();
            var quantityTotals = await _warehouseInventoryRepository.GetWarehouseQuantityTotalsAsync();

            var warehouseList = warehouses
                .Where(w => w.IsActive == true && productCounts.ContainsKey(w.Id) && productCounts[w.Id] > 0)
                .Select(w => new WarehouseListDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    AddressLine1 = w.AddressLine1,
                    AddressLine2 = w.AddressLine2,
                    City = w.City,
                    State = w.State,
                    ZipCode = w.ZipCode,
                    IsActive = w.IsActive,
                    TotalProducts = productCounts.GetValueOrDefault(w.Id, 0),
                    TotalQuantity = quantityTotals.GetValueOrDefault(w.Id, 0)
                }).ToList();

            return warehouseList;
        }

        public async Task<IEnumerable<WarehouseInventoryDetailDto>> GetWarehouseInventoryDetailsAsync(int warehouseId)
        {
            var inventoryItems = await _warehouseInventoryRepository.GetInventoryByWarehouseWithDetailsAsync(warehouseId);

            return inventoryItems.Select(wi => new WarehouseInventoryDetailDto
            {
                Id = wi.Id,
                ProductName = wi.Product.Name,
                ProductSKU = wi.ProductVariant?.BarcodeSKU ?? string.Empty,
                VariantName = wi.ProductVariant?.VariantName,
                VariantSku = wi.ProductVariant?.SKU,
                VariationType = wi.ProductVariant?.Attributes.FirstOrDefault()?.Variation.Name ?? string.Empty,
                VariationValue = wi.ProductVariant?.Attributes.FirstOrDefault()?.VariationOption.Name ?? string.Empty,
                VariationSKU = string.Empty,
                Quantity = wi.Quantity,
                Notes = wi.Notes,
                CreatedAt = wi.CreatedAt,
                UpdatedAt = wi.UpdatedAt
            }).ToList();
        }

        public async Task<WarehouseInventoryDto?> GetInventoryByIdAsync(int id)
        {
            var inventory = await _warehouseInventoryRepository.GetByIdAsync(id);
            if (inventory == null) return null;

            // Load related entities
            var warehouse = await _warehouseRepository.GetByIdAsync(inventory.WarehouseId);
            var product = await _productRepository.GetByIdAsync(inventory.ProductId);
            ProductVariant? productVariant = null;
            if (inventory.ProductVariantId.HasValue)
            {
                productVariant = await _productVariantRepository.GetByIdAsync(inventory.ProductVariantId.Value);
            }

            if (warehouse == null || product == null)
                return null;

            return new WarehouseInventoryDto
            {
                Id = inventory.Id,
                WarehouseId = inventory.WarehouseId,
                WarehouseName = warehouse.Name,
                WarehouseAddress = warehouse.AddressLine1,
                WarehouseCity = warehouse.City,
                WarehouseState = warehouse.State,
                WarehouseZipCode = warehouse.ZipCode,
                ProductId = inventory.ProductId,
                ProductName = product.Name,
                ProductSKU = productVariant?.BarcodeSKU ?? string.Empty,
                ProductVariantId = inventory.ProductVariantId,
                VariantName = productVariant?.VariantName,
                VariationType = productVariant?.Attributes.FirstOrDefault()?.Variation.Name ?? string.Empty,
                VariationValue = productVariant?.Attributes.FirstOrDefault()?.VariationOption.Name ?? string.Empty,
                VariationSKU = string.Empty,
                Quantity = inventory.Quantity,
                Notes = inventory.Notes,
                CreatedAt = inventory.CreatedAt,
                IsActive = inventory.IsActive
            };
        }

        public async Task<WarehouseInventoryDto?> UpdateInventoryAsync(int id, UpdateInventoryDto updateInventoryDto)
        {
            var inventory = await _warehouseInventoryRepository.GetByIdAsync(id);
            if (inventory == null) return null;

            inventory.Quantity = updateInventoryDto.Quantity;
            inventory.Notes = updateInventoryDto.Notes;
            inventory.UpdatedAt = DateTime.UtcNow;

            await _warehouseInventoryRepository.UpdateAsync(inventory);
            return await GetInventoryByIdAsync(id);
        }

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            var inventory = await _warehouseInventoryRepository.GetByIdAsync(id);
            if (inventory == null) return false;

            inventory.IsActive = false;
            inventory.UpdatedAt = DateTime.UtcNow;
            await _warehouseInventoryRepository.UpdateAsync(inventory);
            return true;
        }

        public async Task<IEnumerable<WarehouseInventoryDto>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            var inventoryItems = await _warehouseInventoryRepository.GetInventoryByWarehouseWithDetailsAsync(warehouseId);

            return inventoryItems.Select(wi => new WarehouseInventoryDto
            {
                Id = wi.Id,
                WarehouseId = wi.WarehouseId,
                WarehouseName = wi.Warehouse.Name,
                WarehouseAddress = wi.Warehouse.AddressLine1,
                WarehouseCity = wi.Warehouse.City,
                WarehouseState = wi.Warehouse.State,
                WarehouseZipCode = wi.Warehouse.ZipCode,
                ProductId = wi.ProductId,
                ProductName = wi.Product.Name,
                ProductSKU = wi.ProductVariant?.BarcodeSKU ?? string.Empty,
                ProductVariantId = wi.ProductVariantId,
                VariantName = wi.ProductVariant?.VariantName,
                VariationType = wi.ProductVariant?.Attributes.FirstOrDefault()?.Variation.Name ?? string.Empty,
                VariationValue = wi.ProductVariant?.Attributes.FirstOrDefault()?.VariationOption.Name ?? string.Empty,
                VariationSKU = string.Empty,
                Quantity = wi.Quantity,
                Notes = wi.Notes,
                CreatedAt = wi.CreatedAt,
                IsActive = wi.IsActive
            }).ToList();
        }

        public async Task<IEnumerable<WarehouseInventoryDto>> GetInventoryByProductAsync(int productId)
        {
            var inventoryItems = await _warehouseInventoryRepository.GetByProductIdAsync(productId);

            return inventoryItems.Select(wi => new WarehouseInventoryDto
            {
                Id = wi.Id,
                WarehouseId = wi.WarehouseId,
                WarehouseName = wi.Warehouse.Name,
                WarehouseAddress = wi.Warehouse.AddressLine1,
                WarehouseCity = wi.Warehouse.City,
                WarehouseState = wi.Warehouse.State,
                WarehouseZipCode = wi.Warehouse.ZipCode,
                ProductId = wi.ProductId,
                ProductName = wi.Product.Name,
                ProductSKU = wi.ProductVariant?.BarcodeSKU ?? string.Empty,
                ProductVariantId = wi.ProductVariantId,
                VariantName = wi.ProductVariant?.VariantName,
                VariationType = wi.ProductVariant?.Attributes.FirstOrDefault()?.Variation.Name ?? string.Empty,
                VariationValue = wi.ProductVariant?.Attributes.FirstOrDefault()?.VariationOption.Name ?? string.Empty,
                VariationSKU = string.Empty,
                Quantity = wi.Quantity,
                Notes = wi.Notes,
                CreatedAt = wi.CreatedAt,
                IsActive = wi.IsActive
            }).ToList();
        }

        public async Task<bool> ExistsByWarehouseAndProductVariantAsync(int warehouseId, int? ProductVariantId)
        {
            if (!ProductVariantId.HasValue) return false;
            return await _warehouseInventoryRepository.ExistsByWarehouseAndProductVariantAsync(warehouseId, ProductVariantId.Value);
        }
    }
}
