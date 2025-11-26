using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IRestockRequestService
    {
        Task<IEnumerable<RestockRequestDto>> GetAllRequestsAsync();
        Task<RestockRequestDto?> GetRequestByIdAsync(int id);
        Task<IEnumerable<RestockRequestDto>> GetRequestsByLocationIdAsync(int locationId);
        Task<IEnumerable<RestockRequestDto>> GetRequestsByCustomerIdAsync(int customerId);
        Task<IEnumerable<RestockRequestSummaryDto>> GetRequestsSummaryAsync();
        Task<RestockRequestDto> CreateRequestAsync(CreateRestockRequestDto createDto);
        Task<bool> DeleteRequestAsync(int id);
    }
}
