using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IRequestSupplyService
    {
        Task<IEnumerable<RequestSupplyDto>> GetAllRequestSuppliesAsync();
        Task<RequestSupplyDto?> GetRequestSupplyByIdAsync(int id);
        Task<RequestSupplyDto> CreateRequestSupplyAsync(CreateRequestSupplyDto createRequestSupplyDto);
        Task<RequestSupplyDto?> UpdateRequestSupplyAsync(int id, UpdateRequestSupplyDto updateRequestSupplyDto);
        Task<bool> DeleteRequestSupplyAsync(int id);
        Task<IEnumerable<RequestSupplyDto>> GetRequestSuppliesByStatusAsync(string status);
    }
}
