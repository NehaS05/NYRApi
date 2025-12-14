using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class VanService : IVanService
    {
        private readonly IVanRepository _vanRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VanService(IVanRepository vanRepository, ApplicationDbContext context, IMapper mapper)
        {
            _vanRepository = vanRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VanDto>> GetAllAsync()
        {
            var vans = await _vanRepository.GetAllAsync();
            vans = vans.Where(x => x.IsActive == true);
            return _mapper.Map<IEnumerable<VanDto>>(vans);
        }

        public async Task<VanDto?> GetByIdAsync(int id)
        {
            var van = await _vanRepository.GetByIdAsync(id);
            return van != null ? _mapper.Map<VanDto>(van) : null;
        }

        public async Task<VanDto> CreateAsync(CreateVanDto dto)
        {
            // unique van number
            var existing = await _vanRepository.GetByVanNumberAsync(dto.VanNumber);
            if (existing != null)
                throw new ArgumentException("Van number already exists");

            var entity = _mapper.Map<Van>(dto);
            var created = await _vanRepository.AddAsync(entity);
            return _mapper.Map<VanDto>(created);
        }

        public async Task<VanDto?> UpdateAsync(int id, UpdateVanDto dto)
        {
            var van = await _vanRepository.GetByIdAsync(id);
            if (van == null) return null;

            var existing = await _vanRepository.GetByVanNumberAsync(dto.VanNumber);
            if (existing != null && existing.Id != id)
                throw new ArgumentException("Van number already exists");

            _mapper.Map(dto, van);
            van.UpdatedAt = DateTime.UtcNow;
            await _vanRepository.UpdateAsync(van);
            return _mapper.Map<VanDto>(van);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var van = await _vanRepository.GetByIdAsync(id);
            if (van == null) return false;

            // Check if there are van inventories associated with this van
            var hasInventoriesQuery = _context.VanInventories
                .Where(vi => vi.VanId == id);
            
            var hasInventories = await hasInventoriesQuery.AnyAsync();
            
            if (hasInventories)
            {
                // Soft delete: deactivate the van instead of hard delete
                van.IsActive = false;
                van.UpdatedAt = DateTime.UtcNow;
                await _vanRepository.UpdateAsync(van);
                
                // Optionally deactivate associated van inventories
                var inventories = await hasInventoriesQuery.ToListAsync();
                foreach (var inventory in inventories)
                {
                    // Check if VanInventory has IsActive property, if so deactivate it
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
                await _vanRepository.DeleteAsync(van);
            }
            
            return true;
        }

        public async Task<IEnumerable<VanDto>> SearchAsync(string searchTerm)
        {
            var result = await _vanRepository.SearchAsync(searchTerm);
            return _mapper.Map<IEnumerable<VanDto>>(result);
        }
    }
}


