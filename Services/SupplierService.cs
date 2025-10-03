using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierService(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier != null ? _mapper.Map<SupplierDto>(supplier) : null;
        }

        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto createSupplierDto)
        {
            // Check if supplier with same email already exists
            var existingSupplier = await _supplierRepository.GetByEmailAsync(createSupplierDto.Email);
            if (existingSupplier != null)
                throw new ArgumentException("Supplier with this email already exists");

            var supplier = _mapper.Map<Supplier>(createSupplierDto);
            var createdSupplier = await _supplierRepository.AddAsync(supplier);
            return _mapper.Map<SupplierDto>(createdSupplier);
        }

        public async Task<SupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierDto updateSupplierDto)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return null;

            // Check if another supplier with same email exists
            var existingSupplier = await _supplierRepository.GetByEmailAsync(updateSupplierDto.Email);
            if (existingSupplier != null && existingSupplier.Id != id)
                throw new ArgumentException("Supplier with this email already exists");

            _mapper.Map(updateSupplierDto, supplier);
            supplier.UpdatedAt = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return false;

            await _supplierRepository.DeleteAsync(supplier);
            return true;
        }

        public async Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetActiveSuppliersAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }
    }
}
