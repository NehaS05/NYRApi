using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class ScannerService : IScannerService
    {
        private readonly IScannerRepository _scannerRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public ScannerService(IScannerRepository scannerRepository, ILocationRepository locationRepository, IMapper mapper)
        {
            _scannerRepository = scannerRepository;
            _locationRepository = locationRepository;
            _mapper = mapper;
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

            // Check if ScannerId already exists
            var existingScanner = await _scannerRepository.GetByScannerIdAsync(createScannerDto.ScannerId);
            if (existingScanner != null)
                throw new ArgumentException("Scanner ID already exists");

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

            // Check if ScannerId already exists for another scanner
            var existingScanner = await _scannerRepository.GetByScannerIdAsync(updateScannerDto.ScannerId);
            if (existingScanner != null && existingScanner.Id != id)
                throw new ArgumentException("Scanner ID already exists");

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
    }
}

