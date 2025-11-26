using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class FollowupRequestService : IFollowupRequestService
    {
        private readonly IFollowupRequestRepository _followupRequestRepository;

        public FollowupRequestService(IFollowupRequestRepository followupRequestRepository)
        {
            _followupRequestRepository = followupRequestRepository;
        }

        public async Task<IEnumerable<FollowupRequestDto>> GetAllFollowupRequestsAsync()
        {
            var followupRequests = await _followupRequestRepository.GetAllWithDetailsAsync();
            return followupRequests.Select(MapToDto);
        }

        public async Task<FollowupRequestDto?> GetFollowupRequestByIdAsync(int id)
        {
            var followupRequest = await _followupRequestRepository.GetByIdWithDetailsAsync(id);
            return followupRequest != null ? MapToDto(followupRequest) : null;
        }

        public async Task<IEnumerable<FollowupRequestDto>> GetFollowupRequestsByLocationIdAsync(int locationId)
        {
            var followupRequests = await _followupRequestRepository.GetByLocationIdAsync(locationId);
            return followupRequests.Select(MapToDto);
        }

        public async Task<IEnumerable<FollowupRequestDto>> GetFollowupRequestsByCustomerIdAsync(int customerId)
        {
            var followupRequests = await _followupRequestRepository.GetByCustomerIdAsync(customerId);
            return followupRequests.Select(MapToDto);
        }

        public async Task<IEnumerable<FollowupRequestDto>> GetFollowupRequestsByStatusAsync(string status)
        {
            var followupRequests = await _followupRequestRepository.GetByStatusAsync(status);
            return followupRequests.Select(MapToDto);
        }

        public async Task<FollowupRequestDto> CreateFollowupRequestAsync(CreateFollowupRequestDto createDto)
        {
            var followupRequest = new FollowupRequest
            {
                CustomerId = createDto.CustomerId,
                LocationId = createDto.LocationId,
                Status = "Followup Requested",
                FollowupDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _followupRequestRepository.AddAsync(followupRequest);

            var createdFollowup = await _followupRequestRepository.GetByIdWithDetailsAsync(followupRequest.Id);
            return MapToDto(createdFollowup!);
        }

        public async Task<FollowupRequestDto?> UpdateFollowupRequestStatusAsync(int id, UpdateFollowupRequestDto updateDto)
        {
            var followupRequest = await _followupRequestRepository.GetByIdAsync(id);
            if (followupRequest == null)
                return null;

            followupRequest.Status = updateDto.Status;
            followupRequest.UpdatedAt = DateTime.UtcNow;

            await _followupRequestRepository.UpdateAsync(followupRequest);

            var updatedFollowup = await _followupRequestRepository.GetByIdWithDetailsAsync(id);
            return MapToDto(updatedFollowup!);
        }

        public async Task<bool> DeleteFollowupRequestAsync(int id)
        {
            var followupRequest = await _followupRequestRepository.GetByIdAsync(id);
            if (followupRequest == null)
                return false;

            followupRequest.IsActive = false;
            followupRequest.UpdatedAt = DateTime.UtcNow;

            await _followupRequestRepository.UpdateAsync(followupRequest);

            return true;
        }

        private static FollowupRequestDto MapToDto(FollowupRequest followupRequest)
        {
            return new FollowupRequestDto
            {
                Id = followupRequest.Id,
                CustomerId = followupRequest.CustomerId,
                CustomerName = followupRequest.Customer?.CompanyName ?? "Unknown",
                LocationId = followupRequest.LocationId,
                LocationName = followupRequest.Location?.LocationName ?? "Unknown",
                Status = followupRequest.Status,
                FollowupDate = followupRequest.FollowupDate,
                CreatedAt = followupRequest.CreatedAt,
                UpdatedAt = followupRequest.UpdatedAt,
                IsActive = followupRequest.IsActive
            };
        }
    }
}
