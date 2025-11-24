using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class VanInventoryService : IVanInventoryService
    {
        private readonly IVanInventoryRepository _vanInventoryRepository;
        private readonly IVanRepository _vanRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariationRepository _productVariationRepository;
        private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
        private readonly IMapper _mapper;

        public VanInventoryService(
            IVanInventoryRepository vanInventoryRepository,
            IVanRepository vanRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IProductVariationRepository productVariationRepository,
            IWarehouseInventoryRepository warehouseInventoryRepository,
            IMapper mapper)
        {
            _vanInventoryRepository = vanInventoryRepository;
            _vanRepository = vanRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _productVariationRepository = productVariationRepository;
            _warehouseInventoryRepository = warehouseInventoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VanWithInventorySummaryDto>> GetVansWithTransfersAsync()
        {
            var vans = await _vanRepository.GetAllAsync();
            var vanInventories = await _vanInventoryRepository.GetAllWithDetailsAsync();

            var summary = vans.Where(v => v.IsActive).Select(van => new VanWithInventorySummaryDto
            {
                VanId = van.Id,
                VanName = van.VanName,
                VanNumber = van.VanNumber,
                DriverName = van.DefaultDriverName,
                TotalTransfers = vanInventories.Count(vi => vi.VanId == van.Id),
                TotalItems = vanInventories.Where(vi => vi.VanId == van.Id).Sum(vi => vi.Items.Sum(i => i.Quantity))
            }).ToList();

            return summary;
        }

        public async Task<IEnumerable<VanInventoryDto>> GetAllTransfersAsync()
        {
            var vanInventories = await _vanInventoryRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<VanInventoryDto>>(vanInventories);
        }

        public async Task<VanInventoryDto?> GetTransferByIdAsync(int id)
        {
            var vanInventory = await _vanInventoryRepository.GetByIdWithDetailsAsync(id);
            return vanInventory != null ? _mapper.Map<VanInventoryDto>(vanInventory) : null;
        }

        public async Task<IEnumerable<VanInventoryItemDto>> GetTransferItemsByVanIdAsync(int vanId)
        {
            var vanInventories = await _vanInventoryRepository.GetByVanIdAsync(vanId);
            var allItems = vanInventories.SelectMany(vi => vi.Items);
            return _mapper.Map<IEnumerable<VanInventoryItemDto>>(allItems);
        }

        public async Task<VanInventoryDto> CreateTransferAsync(CreateVanInventoryDto createDto)
        {
            // Validate Van exists
            var van = await _vanRepository.GetByIdAsync(createDto.VanId);
            if (van == null)
                throw new ArgumentException("Van not found");

            // Validate Location exists (if provided)
            if (createDto.LocationId.HasValue)
            {
                var location = await _locationRepository.GetByIdAsync(createDto.LocationId.Value);
                if (location == null)
                    throw new ArgumentException("Location not found");
            }

            // Validate all products and variations exist, and check/deduct from warehouse inventory
            foreach (var item in createDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");

                var variation = await _productVariationRepository.GetByIdAsync(item.ProductVariationId);
                if (variation == null)
                    throw new ArgumentException($"Product variation with ID {item.ProductVariationId} not found");

                if (variation.ProductId != item.ProductId)
                    throw new ArgumentException($"Product variation {item.ProductVariationId} does not belong to product {item.ProductId}");

                // Find the corresponding warehouse inventory item
                var warehouseItem = await _warehouseInventoryRepository.GetByWarehouseAndProductVariationAsync(
                    createDto.WarehouseId, 
                    item.ProductVariationId);

                if (warehouseItem == null)
                {
                    throw new ArgumentException($"Product variation not found in warehouse inventory");
                }

                if (warehouseItem.Quantity < item.Quantity)
                {
                    throw new ArgumentException($"Insufficient quantity in warehouse. Available: {warehouseItem.Quantity}, Requested: {item.Quantity}");
                }

                // Deduct quantity from warehouse inventory
                warehouseItem.Quantity -= item.Quantity;
                warehouseItem.UpdatedAt = DateTime.UtcNow;
                await _warehouseInventoryRepository.UpdateAsync(warehouseItem);
            }

            // Create VanInventory entity
            var entity = _mapper.Map<VanInventory>(createDto);
            
            // Add items
            entity.Items = createDto.Items.Select(itemDto => new VanInventoryItem
            {
                ProductId = itemDto.ProductId,
                ProductVariationId = itemDto.ProductVariationId,
                Quantity = itemDto.Quantity,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            var created = await _vanInventoryRepository.AddAsync(entity);
            
            // Fetch with details for proper mapping
            var result = await _vanInventoryRepository.GetByIdWithDetailsAsync(created.Id);
            return _mapper.Map<VanInventoryDto>(result);
        }

        public async Task<bool> DeleteTransferAsync(int id)
        {
            var vanInventory = await _vanInventoryRepository.GetByIdAsync(id);
            if (vanInventory == null) return false;
            
            vanInventory.IsActive = false;
            vanInventory.UpdatedAt = DateTime.UtcNow;
            await _vanInventoryRepository.UpdateAsync(vanInventory);
            return true;
        }

        public async Task<IEnumerable<TransferTrackingDto>> GetAllTransfersTrackingAsync()
        {
            var vanInventories = await _vanInventoryRepository.GetAllWithDetailsAsync();
            
            var trackingList = vanInventories.Select(vi => new TransferTrackingDto
            {
                Id = vi.Id,
                VanId = vi.VanId,
                VanName = vi.Van?.VanName ?? "Unknown",
                VanNumber = vi.Van?.VanNumber ?? "N/A",
                LocationId = vi.LocationId,
                LocationName = vi.Location?.LocationName ?? "Unknown",
                CustomerId = vi.Location?.CustomerId ?? 0,
                CustomerName = vi.Location?.Customer?.CompanyName ?? "Unknown",
                TransferDate = vi.TransferDate,
                DeliveryDate = vi.DeliveryDate,
                DriverName = vi.DriverName ?? vi.Van?.DefaultDriverName ?? "Not Assigned",
                Status = vi.Status,
                TotalItems = vi.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = vi.CreatedAt,
                UpdatedAt = vi.UpdatedAt
            }).ToList();

            return trackingList;
        }

        public async Task<TransferTrackingDto?> UpdateTransferStatusAsync(int id, UpdateTransferStatusDto updateDto)
        {
            var vanInventory = await _vanInventoryRepository.GetByIdWithDetailsAsync(id);
            if (vanInventory == null)
                return null;

            // Update status and related fields
            vanInventory.Status = updateDto.Status;
            vanInventory.DeliveryDate = updateDto.DeliveryDate;
            
            if (!string.IsNullOrEmpty(updateDto.DriverName))
            {
                vanInventory.DriverName = updateDto.DriverName;
            }

            vanInventory.UpdatedAt = DateTime.UtcNow;
            await _vanInventoryRepository.UpdateAsync(vanInventory);

            // Return updated tracking info
            return new TransferTrackingDto
            {
                Id = vanInventory.Id,
                VanId = vanInventory.VanId,
                VanName = vanInventory.Van?.VanName ?? "Unknown",
                VanNumber = vanInventory.Van?.VanNumber ?? "N/A",
                LocationId = vanInventory.LocationId,
                LocationName = vanInventory.Location?.LocationName ?? "Unknown",
                CustomerId = vanInventory.Location?.CustomerId ?? 0,
                CustomerName = vanInventory.Location?.Customer?.CompanyName ?? "Unknown",
                TransferDate = vanInventory.TransferDate,
                DeliveryDate = vanInventory.DeliveryDate,
                DriverName = vanInventory.DriverName ?? vanInventory.Van?.DefaultDriverName ?? "Not Assigned",
                Status = vanInventory.Status,
                TotalItems = vanInventory.Items?.Sum(i => i.Quantity) ?? 0,
                CreatedAt = vanInventory.CreatedAt,
                UpdatedAt = vanInventory.UpdatedAt
            };
        }
    }
}
