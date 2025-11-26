using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IFollowupRequestService
    {
        Task<IEnumerable<FollowupRequestDto>> GetAllFollowupRequestsAsync();
        Task<FollowupRequestDto?> GetFollowupRequestByIdAsync(int id);
        Task<IEnumerable<FollowupRequestDto>> GetFollowupRequestsByLocationIdAsync(int locationId);
        Task<IEnumerable<FollowupRequestDto>> GetFollowupRequestsByCustomerIdAsync(int customerId);
        Task<IEnumerable<FollowupRequestDto>> GetFollowupRequestsByStatusAsync(string status);
        Task<FollowupRequestDto> CreateFollowupRequestAsync(CreateFollowupRequestDto createDto);
        Task<FollowupRequestDto?> UpdateFollowupRequestStatusAsync(int id, UpdateFollowupRequestDto updateDto);
        Task<bool> DeleteFollowupRequestAsync(int id);
    }
}
