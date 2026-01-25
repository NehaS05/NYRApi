using NYR.API.Models.DTOs;

namespace NYR.API.Services.Interfaces
{
    public interface IScannerService
    {
        Task<IEnumerable<ScannerDto>> GetAllScannersAsync();
        Task<PagedResultDto<ScannerDto>> GetScannersPagedAsync(PaginationParamsDto paginationParams);
        Task<ScannerDto?> GetScannerByIdAsync(int id);
        Task<ScannerDto> CreateScannerAsync(CreateScannerDto createScannerDto);
        Task<ScannerDto?> UpdateScannerAsync(int id, UpdateScannerDto updateScannerDto);
        Task<bool> DeleteScannerAsync(int id);
        Task<IEnumerable<ScannerDto>> GetScannersByLocationAsync(int locationId);
        Task<IEnumerable<ScannerDto>> SearchScannersAsync(string searchTerm);
        Task<ScannerPinConfirmResponseDto> ConfirmScannerPinAsync(ScannerPinConfirmDto confirmDto);
        Task<ScannerPinResetResponseDto> ResetScannerPinAsync(ScannerPinResetDto resetDto);
        Task<ScannerPinConfirmResponseDto?> RefreshScannerTokenAsync(string refreshToken);
        Task<bool> RevokeScannerRefreshTokenAsync(string refreshToken);
    }
}

