using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<UserDto?> RegisterAsync(CreateUserDto createUserDto);
        Task<bool> ValidateUserAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(UserDto user);
        Task<string> GenerateRefreshTokenAsync();
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    }
}
