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
        private readonly ITransferInventoryRepository _transferInventoryRepository;
        private readonly IMapper _mapper;

        public VanInventoryService(
            IVanInventoryRepository vanInventoryRepository,
            IVanRepository vanRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IProductVariationRepository productVariationRepository,
            ITransferInventoryRepository transferInventoryRepository,
            IMapper mapper)
        {
            _vanInventoryRepository = vanInventoryRepository;
            _vanRepository = vanRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _productVariationRepository = productVariationRepository;
            _transferInventoryRepository = transferInventoryRepository;
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

            // Validate Location exists
            var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
            if (location == null)
                throw new ArgumentException("Location not found");

            // Get all transfer inventory items for this location
            var transferInventories = await _transferInventoryRepository.GetByLocationIdAsync(createDto.LocationId);
            var transferInventoryItems = transferInventories.SelectMany(t => t.Items).ToList();

            // Validate all products and variations exist, and check/deduct from transfer inventory
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

                // Find the corresponding transfer inventory item
                var transferItem = transferInventoryItems.FirstOrDefault(ti => 
                    ti.ProductId == item.ProductId && 
                    ti.ProductVariationId == item.ProductVariationId);

                if (transferItem == null)
                {
                    throw new ArgumentException($"Product variation not found in location inventory");
                }

                if (transferItem.Quantity < item.Quantity)
                {
                    throw new ArgumentException($"Insufficient quantity in location. Available: {transferItem.Quantity}, Requested: {item.Quantity}");
                }

                // Deduct quantity from transfer inventory
                transferItem.Quantity -= item.Quantity;
            }

            // Save all updated transfer inventory items and update parent entities
            foreach (var transferInventory in transferInventories)
            {
                transferInventory.UpdatedAt = DateTime.UtcNow;
                await _transferInventoryRepository.UpdateAsync(transferInventory);
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
    }
}
