using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public WarehouseService(IWarehouseRepository warehouseRepository, ApplicationDbContext context, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            warehouses = warehouses.Where(x => x.IsActive);
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id)
        {
            var entity = await _warehouseRepository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<WarehouseDto>(entity) : null;
        }

        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
        {
            var entity = _mapper.Map<Warehouse>(dto);
            var created = await _warehouseRepository.AddAsync(entity);
            return _mapper.Map<WarehouseDto>(created);
        }

        public async Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseDto dto)
        {
            var entity = await _warehouseRepository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _warehouseRepository.UpdateAsync(entity);
            return _mapper.Map<WarehouseDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _warehouseRepository.GetByIdAsync(id);
            if (entity == null) return false;

            // Check if there are warehouse inventories associated with this warehouse
            var hasInventoriesQuery = _context.WarehouseInventories
                .Where(wi => wi.WarehouseId == id);
            
            var hasInventories = await hasInventoriesQuery.AnyAsync();
            
            if (hasInventories)
            {
                // Soft delete: deactivate the warehouse instead of hard delete
                entity.IsActive = false;
                entity.UpdatedAt = DateTime.UtcNow;
                await _warehouseRepository.UpdateAsync(entity);
                
                // Optionally deactivate associated warehouse inventories
                var inventories = await hasInventoriesQuery.ToListAsync();
                foreach (var inventory in inventories)
                {
                    // Check if WarehouseInventory has IsActive property, if so deactivate it
                    var inventoryType = inventory.GetType();
                    var isActiveProperty = inventoryType.GetProperty("IsActive");
                    if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
                    {
                        isActiveProperty.SetValue(inventory, false);
                    }
                    
                    var updatedAtProperty = inventoryType.GetProperty("UpdatedAt");
                    if (updatedAtProperty != null && updatedAtProperty.PropertyType == typeof(DateTime?))
                    {
                        updatedAtProperty.SetValue(inventory, DateTime.UtcNow);
                    }
                }
                
                await _context.SaveChangesAsync();
            }
            else
            {
                // No inventories, safe to hard delete
                await _warehouseRepository.DeleteAsync(entity);
            }
            
            return true;
        }

        public async Task<IEnumerable<WarehouseDto>> SearchAsync(string searchTerm)
        {
            var result = await _warehouseRepository.SearchAsync(searchTerm);
            result = result.Where(x => x.IsActive);
            return _mapper.Map<IEnumerable<WarehouseDto>>(result);
        }
    }
}


