using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NYR.API.Data;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NYR.API.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationInventoryDataRepository _locationInventoryDataRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITransferInventoryRepository _transferInventoryRepository;
        private readonly IRestockRequestRepository _restockRequestRepository;
        private readonly IFollowupRequestRepository _followupRequestRepository;
        private readonly IScannerRepository _scannerRepository;
        private readonly IConfiguration _configuration;

        public LocationService(ILocationRepository locationRepository, ICustomerRepository customerRepository, IUserRepository userRepository, ILocationInventoryDataRepository locationInventoryDataRepository, ApplicationDbContext context, IMapper mapper, ITransferInventoryRepository transferInventoryRepository, IRestockRequestRepository restockRequestRepository, IFollowupRequestRepository followupRequestRepository, IScannerRepository scannerRepository, IConfiguration configuration)
        {
            _locationRepository = locationRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _locationInventoryDataRepository = locationInventoryDataRepository;
            _context = context;
            _mapper = mapper;
            _transferInventoryRepository = transferInventoryRepository;
            _restockRequestRepository = restockRequestRepository;
            _followupRequestRepository = followupRequestRepository;
            _scannerRepository = scannerRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            locations = locations.Where(x => x.IsActive == true);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<PagedResultDto<LocationDto>> GetLocationsPagedAsync(PaginationParamsDto paginationParams)
        {
            PaginationServiceHelper.NormalizePaginationParams(paginationParams);

            var (items, totalCount) = await _locationRepository.GetPagedAsync(paginationParams);
            var locationDtos = _mapper.Map<IEnumerable<LocationDto>>(items);

            return PaginationServiceHelper.CreatePagedResult(locationDtos, totalCount, paginationParams);
        }
        public async Task<IEnumerable<LocationDto>> GetAllLocationsWithInventoryAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            var locationDtos = _mapper.Map<IEnumerable<LocationDto>>(locations).ToList();
            
            foreach (var locationDto in locationDtos)
            {
                try
                {
                    // Get restock requests for this location
                    var restockRequests = await _restockRequestRepository.GetByLocationIdAsync(locationDto.Id);
                    if (restockRequests != null && restockRequests.Any())
                    {
                        // Flatten all items from all restock requests for this location
                        var allItems = restockRequests.SelectMany(rr => rr.Items).ToList();
                        if (allItems.Any())
                        {
                            // Map RestockRequestItems to TransferInventoryItemDto
                            locationDto.TransferItems = allItems.Select(item => new TransferInventoryItemDto
                            {
                                ProductId = item.ProductId,
                                ProductName = item.Product?.Name ?? string.Empty,
                                SkuCode = item.ProductVariant?.BarcodeSKU,
                                ProductVariantId = item.ProductVariantId,
                                VariantName = item.ProductVariant?.VariantName,
                                Quantity = item.Quantity,
                                RestockRequestId = item.RestockRequestId
                            }).ToList();
                        }
                    }

                    // Get location inventory data for this location
                    var locationInventoryData = await _locationInventoryDataRepository.GetByLocationIdAsync(locationDto.Id);
                    if (locationInventoryData != null && locationInventoryData.Any())
                    {
                        locationDto.LocationInventoryData = locationInventoryData.Select(inventory => new LocationInventoryDataDto
                        {
                            Id = inventory.Id,
                            LocationId = inventory.LocationId,
                            LocationName = inventory.Location?.LocationName ?? string.Empty,
                            ProductId = inventory.ProductId,
                            ProductName = inventory.Product?.Name ?? string.Empty,
                            ProductSKU = inventory.ProductVariant?.BarcodeSKU ?? string.Empty,
                            ProductVariantId = inventory.ProductVariantId,
                            Quantity = inventory.Quantity,
                            VariantName = inventory.VariationName,
                            CreatedAt = inventory.CreatedAt,
                            CreatedBy = inventory.CreatedBy,
                            CreatedByName = inventory.CreatedByUser?.Name ?? string.Empty,
                            UpdatedBy = inventory.UpdatedBy,
                            UpdatedByName = inventory.UpdatedByUser?.Name,
                            UpdatedDate = inventory.UpdatedDate
                        }).ToList();
                    }
                }
                catch (Exception)
                {
                    // If there's an error loading inventory data for this location, just skip it
                    locationDto.TransferItems = new List<TransferInventoryItemDto>();
                    locationDto.LocationInventoryData = new List<LocationInventoryDataDto>();
                }
            }
            
            return locationDtos;
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int id)
        {
            var location = await _locationRepository.GetLocationWithCustomerAsync(id);
            return location != null ? _mapper.Map<LocationDto>(location) : null;
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(createLocationDto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Invalid customer ID");

            // Validate user exists if UserId is provided
            if (createLocationDto.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(createLocationDto.UserId.Value);
                if (user == null)
                    throw new ArgumentException("Invalid user ID");
            }

            var location = _mapper.Map<Location>(createLocationDto);
            var createdLocation = await _locationRepository.AddAsync(location);
            return _mapper.Map<LocationDto>(createdLocation);
        }

        public async Task<LocationDto?> UpdateLocationAsync(int id, UpdateLocationDto updateLocationDto)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                return null;

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(updateLocationDto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Invalid customer ID");

            // Validate user exists if UserId is provided
            if (updateLocationDto.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(updateLocationDto.UserId.Value);
                if (user == null)
                    throw new ArgumentException("Invalid user ID");
            }

            _mapper.Map(updateLocationDto, location);
            location.UpdatedAt = DateTime.UtcNow;

            await _locationRepository.UpdateAsync(location);
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                return false;

            // Check if there are active scanners associated with this location
            var hasActiveScannersQuery = _context.Scanners
                .Where(s => s.LocationId == id && s.IsActive);
            
            var hasActiveScanners = await hasActiveScannersQuery.AnyAsync();
            
            // Check if there are route stops associated with this location
            var hasRouteStopsQuery = _context.RouteStops
                .Where(rs => rs.LocationId == id && rs.IsActive);
            
            var hasRouteStops = await hasRouteStopsQuery.AnyAsync();
            
            if (hasActiveScanners || hasRouteStops)
            {
                // Soft delete: deactivate the location instead of hard delete
                location.IsActive = false;
                location.UpdatedAt = DateTime.UtcNow;
                await _locationRepository.UpdateAsync(location);
                
                // Also deactivate associated scanners
                if (hasActiveScanners)
                {
                    var scanners = await hasActiveScannersQuery.ToListAsync();
                    foreach (var scanner in scanners)
                    {
                        scanner.IsActive = false;
                        scanner.UpdatedAt = DateTime.UtcNow;
                    }
                }
                
                // Also deactivate associated route stops
                if (hasRouteStops)
                {
                    var routeStops = await hasRouteStopsQuery.ToListAsync();
                    foreach (var routeStop in routeStops)
                    {
                        routeStop.IsActive = false;
                        //routeStop.UpdatedAt = DateTime.UtcNow;
                    }
                }
                
                await _context.SaveChangesAsync();
            }
            else
            {
                // No active scanners or route stops, safe to hard delete
                await _locationRepository.DeleteAsync(location);
            }
            
            return true;
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsByCustomerAsync(int customerId)
        {
            var locations = await _locationRepository.GetLocationsByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<IEnumerable<LocationDto>> SearchLocationsAsync(string searchTerm)
        {
            var locations = await _locationRepository.SearchLocationsAsync(searchTerm);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsNeedingFollowUpAsync()
        {
            var currentDate = DateTime.UtcNow.Date;
            
            // Get locations with FollowUpDays > 0 and have inventory data
            var locationsNeedingFollowUp = await _context.Locations
                .Include(l => l.Customer)
                .Include(l => l.User)
                .Where(l => l.IsActive && l.FollowUpDays.HasValue && l.FollowUpDays > 0)
                .ToListAsync();

            var result = new List<LocationDto>();

            foreach (var location in locationsNeedingFollowUp)
            {
                // Check if location has inventory data
                var hasInventoryData = await _context.LocationInventoryData
                    .AnyAsync(lid => lid.LocationId == location.Id);

                if (!hasInventoryData)
                    continue;

                // Get the last delivery date for this location from Routes
                var lastDeliveryDate = await _context.RouteStops
                    .Include(rs => rs.Route)
                    .Where(rs => rs.LocationId == location.Id && rs.IsActive && rs.Route.Status.ToLower() == "completed")
                    .Select(rs => rs.Route.DeliveryDate)
                    .OrderByDescending(deliveryDate => deliveryDate)
                    .FirstOrDefaultAsync();

                // If no delivery found, skip this location
                if (lastDeliveryDate == default(DateTime))
                    continue;

                // Calculate days since last delivery
                var daysSinceLastDelivery = (currentDate - lastDeliveryDate.Date).Days;

                // Check if follow-up is needed (days since last delivery > follow-up days)
                if (daysSinceLastDelivery > location.FollowUpDays.Value)
                {
                    var locationDto = _mapper.Map<LocationDto>(location);
                    
                    // Create followup request
                    try
                    {
                        var createFollowupDto = new FollowupRequest
                        {
                            CustomerId = location.CustomerId,
                            LocationId = location.Id,
                            Status = "Followup",
                            FollowupDate = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                        
                        await _followupRequestRepository.AddAsync(createFollowupDto);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle as needed
                        // Continue processing even if followup creation fails
                    }
                    
                    // Add additional information about the follow-up
                    locationDto.Comments = $"Last delivery: {lastDeliveryDate:yyyy-MM-dd}, Days since: {daysSinceLastDelivery}, Follow-up needed after: {location.FollowUpDays} days";
                    
                    // Get location inventory data for this location
                    try
                    {
                        var locationInventoryData = await _locationInventoryDataRepository.GetByLocationIdAsync(location.Id);
                        if (locationInventoryData != null && locationInventoryData.Any())
                        {
                            locationDto.LocationInventoryData = locationInventoryData.Select(inventory => new LocationInventoryDataDto
                            {
                                Id = inventory.Id,
                                LocationId = inventory.LocationId,
                                LocationName = inventory.Location?.LocationName ?? string.Empty,
                                ProductId = inventory.ProductId,
                                ProductName = inventory.Product?.Name ?? string.Empty,
                                ProductSKU = inventory.ProductVariant?.BarcodeSKU ?? string.Empty,
                                ProductVariantId = inventory.ProductVariantId,
                                Quantity = inventory.Quantity,
                                VariantName = inventory.VariationName,
                                CreatedAt = inventory.CreatedAt,
                                CreatedBy = inventory.CreatedBy,
                                CreatedByName = inventory.CreatedByUser?.Name ?? string.Empty,
                                UpdatedBy = inventory.UpdatedBy,
                                UpdatedByName = inventory.UpdatedByUser?.Name,
                                UpdatedDate = inventory.UpdatedDate
                            }).ToList();
                        }
                    }
                    catch (Exception)
                    {
                        locationDto.LocationInventoryData = new List<LocationInventoryDataDto>();
                    }

                    result.Add(locationDto);
                }
            }

            return result.OrderByDescending(l => l.FollowUpDays);
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsWithoutScannersAsync()
        {
            var locations = await _locationRepository.GetLocationsWithoutScannersAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<ScannerLocationCheckDto> CheckScannerLocationAssignmentAsync(string serialNo)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return new ScannerLocationCheckDto
                {
                    IsLocation = false,
                    SerialNo = serialNo,
                    Message = "Serial number is required",
                    AvailableLocations = new List<SimpleLocationDto>()
                };
            }

            // First, check if scanner exists and is active
            var scanner = await _scannerRepository.GetBySerialNoAsync(serialNo);
            
            if (scanner == null)
            {
                return new ScannerLocationCheckDto
                {
                    IsLocation = false,
                    SerialNo = serialNo,
                    Message = $"Scanner {serialNo} not found",
                    AvailableLocations = new List<SimpleLocationDto>()
                };
            }

            if (!scanner.IsActive)
            {
                return new ScannerLocationCheckDto
                {
                    IsLocation = false,
                    SerialNo = serialNo,
                    Message = $"Scanner {serialNo} is not active",
                    AvailableLocations = new List<SimpleLocationDto>()
                };
            }

            // Generate tokens for valid scanner
            var token = GenerateScannerJwtToken(scanner);
            var refreshToken = GenerateRefreshToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(24);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(30);

            // Update scanner with new refresh token
            scanner.RefreshToken = refreshToken;
            scanner.RefreshTokenExpiry = refreshTokenExpiry;
            scanner.UpdatedAt = DateTime.UtcNow;
            await _scannerRepository.UpdateAsync(scanner);

            // Check if scanner is assigned to a location
            var assignedLocation = await _locationRepository.GetLocationByScannerSerialNoAsync(serialNo);

            if (assignedLocation != null)
            {
                // Scanner is assigned to a location
                return new ScannerLocationCheckDto
                {
                    IsLocation = true,
                    SerialNo = serialNo,
                    AssignedLocation = _mapper.Map<SimpleLocationDto>(assignedLocation),
                    Message = $"Scanner {serialNo} is assigned to location: {assignedLocation.LocationName}",
                    AvailableLocations = new List<SimpleLocationDto>(),
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpiry = tokenExpiry,
                    RefreshTokenExpiry = refreshTokenExpiry
                };
            }
            else
            {
                // Scanner is not assigned, get available locations
                var availableLocations = await _locationRepository.GetLocationsWithoutScannersAsync();
                
                return new ScannerLocationCheckDto
                {
                    IsLocation = false,
                    SerialNo = serialNo,
                    Message = $"Scanner {serialNo} is not assigned to any location",
                    AvailableLocations = _mapper.Map<IEnumerable<SimpleLocationDto>>(availableLocations),
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpiry = tokenExpiry,
                    RefreshTokenExpiry = refreshTokenExpiry
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, scanner.Id.ToString()),
                new Claim(ClaimTypes.Name, scanner.ScannerName),
                new Claim(ClaimTypes.Role, "Scanner"), // Special role for scanners
                new Claim("SerialNo", scanner.SerialNo),
                new Claim("ScannerType", "Device") // Identify this as a scanner token
            };

            // Only add LocationId claim if scanner has a location assigned
            if (scanner.LocationId.HasValue)
            {
                claims.Add(new Claim("LocationId", scanner.LocationId.Value.ToString()));
            }

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

        public async Task<AssignScannerToLocationResponseDto> AssignScannerToLocationAsync(AssignScannerToLocationDto assignDto)
        {
            try
            {
                // Verify admin PIN first
                if (assignDto.AdminPIN != "0000")
                {
                    return new AssignScannerToLocationResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid admin PIN. Access denied.",
                        SerialNo = assignDto.SerialNo,
                        LocationId = assignDto.LocationId
                    };
                }

                // Validate scanner exists and is active
                var scanner = await _scannerRepository.GetBySerialNoAsync(assignDto.SerialNo);
                if (scanner == null)
                {
                    return new AssignScannerToLocationResponseDto
                    {
                        IsSuccess = false,
                        Message = $"Scanner with serial number {assignDto.SerialNo} not found",
                        SerialNo = assignDto.SerialNo,
                        LocationId = assignDto.LocationId
                    };
                }

                if (!scanner.IsActive)
                {
                    return new AssignScannerToLocationResponseDto
                    {
                        IsSuccess = false,
                        Message = $"Scanner {assignDto.SerialNo} is not active",
                        SerialNo = assignDto.SerialNo,
                        LocationId = assignDto.LocationId
                    };
                }

                // Validate location exists and is active
                var location = await _locationRepository.GetByIdAsync(assignDto.LocationId);
                if (location == null)
                {
                    return new AssignScannerToLocationResponseDto
                    {
                        IsSuccess = false,
                        Message = $"Location with ID {assignDto.LocationId} not found",
                        SerialNo = assignDto.SerialNo,
                        LocationId = assignDto.LocationId
                    };
                }

                if (!location.IsActive)
                {
                    return new AssignScannerToLocationResponseDto
                    {
                        IsSuccess = false,
                        Message = $"Location {location.LocationName} is not active",
                        SerialNo = assignDto.SerialNo,
                        LocationId = assignDto.LocationId,
                        LocationName = location.LocationName
                    };
                }

                // Check if another scanner is already assigned to this location
                var existingScanner = await _scannerRepository.GetScannersByLocationAsync(assignDto.LocationId);
                var otherScanner = existingScanner.FirstOrDefault(s => s.SerialNo != assignDto.SerialNo);
                if (otherScanner != null)
                {
                    return new AssignScannerToLocationResponseDto
                    {
                        IsSuccess = false,
                        Message = $"Location {location.LocationName} already has scanner {otherScanner.SerialNo} assigned",
                        SerialNo = assignDto.SerialNo,
                        LocationId = assignDto.LocationId,
                        LocationName = location.LocationName
                    };
                }

                // Update scanner with new location
                scanner.LocationId = assignDto.LocationId;
                scanner.UpdatedAt = DateTime.UtcNow;
                await _scannerRepository.UpdateAsync(scanner);

                return new AssignScannerToLocationResponseDto
                {
                    IsSuccess = true,
                    Message = $"Scanner {assignDto.SerialNo} successfully assigned to location {location.LocationName}",
                    SerialNo = assignDto.SerialNo,
                    LocationId = assignDto.LocationId,
                    LocationName = location.LocationName
                };
            }
            catch (Exception ex)
            {
                return new AssignScannerToLocationResponseDto
                {
                    IsSuccess = false,
                    Message = $"An error occurred while assigning scanner: {ex.Message}",
                    SerialNo = assignDto.SerialNo,
                    LocationId = assignDto.LocationId
                };
            }
        }
    }
}
