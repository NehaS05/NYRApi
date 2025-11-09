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
        private readonly IGenericRepository<ProductVariation> _productVariationRepository;
        private readonly IMapper _mapper;

        public WarehouseInventoryService(
            IWarehouseInventoryRepository warehouseInventoryRepository,
            IGenericRepository<Warehouse> warehouseRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductVariation> productVariationRepository,
            IMapper mapper)
        {
            _warehouseInventoryRepository = warehouseInventoryRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _productVariationRepository = productVariationRepository;
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

            // Validate product variation exists
            var productVariation = await _productVariationRepository.GetByIdAsync(addInventoryDto.ProductVariationId);
            if (productVariation == null)
                throw new ArgumentException("Product variation not found");

            // Check if inventory already exists for this warehouse and product variation
            var existingInventory = await _warehouseInventoryRepository
                .GetByWarehouseAndProductVariationAsync(addInventoryDto.WarehouseId, addInventoryDto.ProductVariationId);

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
                    ProductVariationId = addInventoryDto.ProductVariationId,
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
                // Validate product variation exists
                var productVariation = await _productVariationRepository.GetByIdAsync(item.ProductVariationId);
                if (productVariation == null)
                    throw new ArgumentException($"Product variation with ID {item.ProductVariationId} not found");

                // Check if inventory already exists for this warehouse and product variation
                var existingInventory = await _warehouseInventoryRepository
                    .GetByWarehouseAndProductVariationAsync(addBulkInventoryDto.WarehouseId, item.ProductVariationId);

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
                        ProductVariationId = item.ProductVariationId,
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
            var productCounts = await _warehouseInventoryRepository.GetWarehouseProductCountsAsync();
            var quantityTotals = await _warehouseInventoryRepository.GetWarehouseQuantityTotalsAsync();

            var warehouseList = warehouses.Select(w => new WarehouseListDto
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
                ProductSKU = wi.Product.BarcodeSKU ?? string.Empty,
                VariationType = wi.ProductVariation.VariationType,
                VariationValue = wi.ProductVariation.VariationValue,
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
            var productVariation = await _productVariationRepository.GetByIdAsync(inventory.ProductVariationId);

            if (warehouse == null || product == null || productVariation == null)
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
                ProductSKU = product.BarcodeSKU ?? string.Empty,
                ProductVariationId = inventory.ProductVariationId,
                VariationType = productVariation.VariationType,
                VariationValue = productVariation.VariationValue,
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
                ProductSKU = wi.Product.BarcodeSKU ?? string.Empty,
                ProductVariationId = wi.ProductVariationId,
                VariationType = wi.ProductVariation.VariationType,
                VariationValue = wi.ProductVariation.VariationValue,
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
                ProductSKU = wi.Product.BarcodeSKU ?? string.Empty,
                ProductVariationId = wi.ProductVariationId,
                VariationType = wi.ProductVariation.VariationType,
                VariationValue = wi.ProductVariation.VariationValue,
                VariationSKU = string.Empty,
                Quantity = wi.Quantity,
                Notes = wi.Notes,
                CreatedAt = wi.CreatedAt,
                IsActive = wi.IsActive
            }).ToList();
        }

        public async Task<bool> ExistsByWarehouseAndProductVariationAsync(int warehouseId, int productVariationId)
        {
            return await _warehouseInventoryRepository.ExistsByWarehouseAndProductVariationAsync(warehouseId, productVariationId);
        }
    }
}
