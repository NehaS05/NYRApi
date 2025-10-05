using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public WarehouseService(IWarehouseRepository warehouseRepository, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
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
            await _warehouseRepository.DeleteAsync(entity);
            return true;
        }

        public async Task<IEnumerable<WarehouseDto>> SearchAsync(string searchTerm)
        {
            var result = await _warehouseRepository.SearchAsync(searchTerm);
            return _mapper.Map<IEnumerable<WarehouseDto>>(result);
        }
    }
}


