using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<SupplierDto?> GetSupplierByIdAsync(int id);
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto createSupplierDto);
        Task<SupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierDto updateSupplierDto);
        Task<bool> DeleteSupplierAsync(int id);
        Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync();
    }
}
