using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NYR.API.Services
{
    public class ScannerService : IScannerService
    {
        private readonly IScannerRepository _scannerRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ScannerService(IScannerRepository scannerRepository, ILocationRepository locationRepository, IMapper mapper, IConfiguration configuration)
        {
            _scannerRepository = scannerRepository;
            _locationRepository = locationRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ScannerDto>> GetAllScannersAsync()
        {
            var scanners = await _scannerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ScannerDto>>(scanners);
        }

        public async Task<ScannerDto?> GetScannerByIdAsync(int id)
        {
            var scanner = await _scannerRepository.GetScannerWithLocationAsync(id);
            return scanner != null ? _mapper.Map<ScannerDto>(scanner) : null;
        }

        public async Task<ScannerDto> CreateScannerAsync(CreateScannerDto createScannerDto)
        {
            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(createScannerDto.LocationId);
            if (location == null)
                throw new ArgumentException("Invalid location ID");

            // Check if SerialNo already exists
            var existingScanner = await _scannerRepository.GetBySerialNoAsync(createScannerDto.SerialNo);
            if (existingScanner != null)
                throw new ArgumentException("Serial Number already exists");

            var scanner = _mapper.Map<Scanner>(createScannerDto);
            var createdScanner = await _scannerRepository.AddAsync(scanner);
            return _mapper.Map<ScannerDto>(createdScanner);
        }

        public async Task<ScannerDto?> UpdateScannerAsync(int id, UpdateScannerDto updateScannerDto)
        {
            var scanner = await _scannerRepository.GetByIdAsync(id);
            if (scanner == null)
                return null;

            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(updateScannerDto.LocationId);
            if (location == null)
                throw new ArgumentException("Invalid location ID");

            // Check if SerialNo already exists for another scanner
            var existingScanner = await _scannerRepository.GetBySerialNoAsync(updateScannerDto.SerialNo);
            if (existingScanner != null && existingScanner.Id != id)
                throw new ArgumentException("Serial Number already exists");

            _mapper.Map(updateScannerDto, scanner);
            scanner.UpdatedAt = DateTime.UtcNow;

            await _scannerRepository.UpdateAsync(scanner);
            return _mapper.Map<ScannerDto>(scanner);
        }

        public async Task<bool> DeleteScannerAsync(int id)
        {
            var scanner = await _scannerRepository.GetByIdAsync(id);
            if (scanner == null)
                return false;

            await _scannerRepository.DeleteAsync(scanner);
            return true;
        }

        public async Task<IEnumerable<ScannerDto>> GetScannersByLocationAsync(int locationId)
        {
            var scanners = await _scannerRepository.GetScannersByLocationAsync(locationId);
            return _mapper.Map<IEnumerable<ScannerDto>>(scanners);
        }

        public async Task<IEnumerable<ScannerDto>> SearchScannersAsync(string searchTerm)
        {
            var scanners = await _scannerRepository.SearchScannersAsync(searchTerm);
            return _mapper.Map<IEnumerable<ScannerDto>>(scanners);
        }

        public async Task<ScannerPinConfirmResponseDto> ConfirmScannerPinAsync(ScannerPinConfirmDto confirmDto)
        {
            // Find scanner by serial number
            var scanner = await _scannerRepository.GetBySerialNoAsync(confirmDto.SerialNo);
            
            if (scanner == null)
            {
                return new ScannerPinConfirmResponseDto
                {
                    IsValid = false,
                    Message = "Scanner not found with the provided serial number",
                    Scanner = null
                };
            }

            // Check if scanner is active
            if (!scanner.IsActive)
            {
                return new ScannerPinConfirmResponseDto
                {
                    IsValid = false,
                    Message = "Scanner is not active",
                    Scanner = null
                };
            }

            // Verify PIN
            if (scanner.ScannerPIN != confirmDto.ScannerPIN)
            {
                return new ScannerPinConfirmResponseDto
                {
                    IsValid = false,
                    Message = "Invalid PIN",
                    Scanner = null
                };
            }

            // PIN is valid - capture the current AppPinReset value before resetting it
            var currentAppPinReset = scanner.AppPinReset;
            
            // Generate tokens
            var token = GenerateScannerJwtToken(scanner);
            var refreshToken = GenerateRefreshToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(24);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(30);

            // Store refresh token in database and reset AppPinReset flag
            scanner.RefreshToken = refreshToken;
            scanner.RefreshTokenExpiry = refreshTokenExpiry;
            scanner.AppPinReset = false; // Reset the flag when PIN is successfully confirmed
            scanner.UpdatedAt = DateTime.UtcNow;
            await _scannerRepository.UpdateAsync(scanner);

            return new ScannerPinConfirmResponseDto
            {
                IsValid = true,
                Message = "PIN confirmed successfully",
                Scanner = _mapper.Map<ScannerDto>(scanner),
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = tokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry
            };
        }

        public async Task<ScannerPinResetResponseDto> ResetScannerPinAsync(ScannerPinResetDto resetDto)
        {
            // Find scanner by serial number
            var scanner = await _scannerRepository.GetBySerialNoAsync(resetDto.SerialNo);
            
            if (scanner == null)
            {
                return new ScannerPinResetResponseDto
                {
                    IsSuccess = false,
                    Message = "Scanner not found with the provided serial number"
                };
            }

            // Check if scanner is active
            if (!scanner.IsActive)
            {
                return new ScannerPinResetResponseDto
                {
                    IsSuccess = false,
                    Message = "Scanner is not active"
                };
            }

            // Update the PIN
            scanner.ScannerPIN = resetDto.NewPIN;
            scanner.UpdatedAt = DateTime.UtcNow;
            scanner.AppPinReset = true;

            try
            {
                await _scannerRepository.UpdateAsync(scanner);
                
                return new ScannerPinResetResponseDto
                {
                    IsSuccess = true,
                    Message = "PIN reset successfully"
                };
            }
            catch (Exception ex)
            {
                return new ScannerPinResetResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to reset PIN. Please try again."
                };
            }
        }

        private string GenerateScannerJwtToken(Scanner scanner)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, scanner.Id.ToString()),
                new Claim(ClaimTypes.Name, scanner.ScannerName),
                new Claim(ClaimTypes.Role, "Scanner"), // Special role for scanners
                new Claim("SerialNo", scanner.SerialNo),
                new Claim("LocationId", scanner.LocationId.ToString()),
                new Claim("ScannerType", "Device") // Identify this as a scanner token
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ScannerPinConfirmResponseDto?> RefreshScannerTokenAsync(string refreshToken)
        {
            var scanner = await _scannerRepository.GetByRefreshTokenAsync(refreshToken);
            if (scanner == null || scanner.RefreshTokenExpiry <= DateTime.UtcNow || !scanner.IsActive)
                return null;

            // Generate new tokens
            var newToken = GenerateScannerJwtToken(scanner);
            var newRefreshToken = GenerateRefreshToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(24);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(30);

            // Update refresh token in database
            scanner.RefreshToken = newRefreshToken;
            scanner.RefreshTokenExpiry = refreshTokenExpiry;
            scanner.UpdatedAt = DateTime.UtcNow;
            await _scannerRepository.UpdateAsync(scanner);

            return new ScannerPinConfirmResponseDto
            {
                IsValid = true,
                Message = "Token refreshed successfully",
                AppPinReset = scanner.AppPinReset,
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpiry = tokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry
            };
        }

        public async Task<bool> RevokeScannerRefreshTokenAsync(string refreshToken)
        {
            var scanner = await _scannerRepository.GetByRefreshTokenAsync(refreshToken);
            if (scanner == null)
                return false;

            scanner.RefreshToken = null;
            scanner.RefreshTokenExpiry = null;
            scanner.UpdatedAt = DateTime.UtcNow;
            await _scannerRepository.UpdateAsync(scanner);
            return true;
        }

    }
}

