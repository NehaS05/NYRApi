using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleId);
        Task<IEnumerable<UserDto>> GetUsersByCustomerAsync(int customerId);
        Task<IEnumerable<UserDto>> GetUsersByLocationAsync(int locationId);
        Task<IEnumerable<DriverAvailabilityDto>> GetDriverAvailabilityAsync(int userId);
        Task<bool> SaveDriverAvailabilityAsync(int userId, DriverAvailabilityBulkDto bulkDto);
        Task<bool> DeleteDriverAvailabilityAsync(int userId, int availabilityId);
    }
}
