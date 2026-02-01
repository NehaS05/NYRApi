using AutoMapper;
using NYR.API.Helpers;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;
using System.Security.Cryptography;

namespace NYR.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IDriverAvailabilityRepository _driverAvailabilityRepository;
        private readonly IMapper _mapper;
        private readonly IFileUploadService _fileUploadService;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ICustomerRepository customerRepository,
            ILocationRepository locationRepository,
            IWarehouseRepository warehouseRepository,
            IDriverAvailabilityRepository driverAvailabilityRepository,
            IMapper mapper,
            IFileUploadService fileUploadService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _customerRepository = customerRepository;
            _locationRepository = locationRepository;
            _warehouseRepository = warehouseRepository;
            _driverAvailabilityRepository = driverAvailabilityRepository;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<PagedResultDto<UserDto>> GetUsersPagedAsync(PaginationParamsDto paginationParams)
        {
            PaginationServiceHelper.NormalizePaginationParams(paginationParams);

            var (items, totalCount) = await _userRepository.GetPagedAsync(paginationParams);
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(items);

            return PaginationServiceHelper.CreatePagedResult(userDtos, totalCount, paginationParams);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(id);
            if (user == null)
                return null;

            var userDto = _mapper.Map<UserDto>(user);
            
            // Load driver availability if user is a driver
            if (user.RoleId == 3) // Driver role
            {
                var availabilities = await _driverAvailabilityRepository.GetActiveByUserIdAsync(id);
                userDto.DriverAvailabilities = _mapper.Map<IEnumerable<DriverAvailabilityDto>>(availabilities);
            }

            return userDto;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(createUserDto.RoleId);
            if (role == null)
                throw new ArgumentException("Invalid role ID");

            // Validate customer exists if provided
            if (createUserDto.CustomerId.HasValue && createUserDto.CustomerId.Value > 0)
            {
                var customer = await _customerRepository.GetByIdAsync(createUserDto.CustomerId.Value);
                if (customer == null)
                    throw new ArgumentException("Invalid customer ID");
            }

            // Validate location exists if provided
            if (createUserDto.LocationId.HasValue && createUserDto.LocationId.Value > 0)
            {
                var location = await _locationRepository.GetByIdAsync(createUserDto.LocationId.Value);
                if (location == null)
                    throw new ArgumentException("Invalid location ID");
            }

            // Validate warehouse exists if provided
            if (createUserDto.WarehouseId.HasValue && createUserDto.WarehouseId.Value > 0)
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(createUserDto.WarehouseId.Value);
                if (warehouse == null)
                    throw new ArgumentException("Invalid warehouse ID");
            }

            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingUser != null)
                throw new ArgumentException("Email already exists");

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = HashPassword(createUserDto.Password);

            var createdUser = await _userRepository.AddAsync(user);
            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(updateUserDto.RoleId);
            if (role == null)
                throw new ArgumentException("Invalid role ID");

            // Validate customer exists if provided
            if (updateUserDto.CustomerId.HasValue)
            {
                var customer = await _customerRepository.GetByIdAsync(updateUserDto.CustomerId.Value);
                if (customer == null)
                    throw new ArgumentException("Invalid customer ID");
            }

            // Validate location exists if provided
            if (updateUserDto.LocationId.HasValue)
            {
                var location = await _locationRepository.GetByIdAsync(updateUserDto.LocationId.Value);
                if (location == null)
                    throw new ArgumentException("Invalid location ID");
            }

            // Validate warehouse exists if provided
            if (updateUserDto.WarehouseId.HasValue)
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(updateUserDto.WarehouseId.Value);
                if (warehouse == null)
                    throw new ArgumentException("Invalid warehouse ID");
            }

            // Check if email already exists for another user
            var existingUser = await _userRepository.GetByEmailAsync(updateUserDto.Email);
            if (existingUser != null && existingUser.Id != id)
                throw new ArgumentException("Email already exists");

            _mapper.Map(updateUserDto, user);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleId)
        {
            var users = await _userRepository.GetUsersByRoleAsync(roleId);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<IEnumerable<UserDto>> GetUsersByCustomerAsync(int customerId)
        {
            var users = await _userRepository.GetUsersByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<IEnumerable<UserDto>> GetUsersByLocationAsync(int locationId)
        {
            var users = await _userRepository.GetUsersByLocationAsync(locationId);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        private string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        public async Task<IEnumerable<DriverAvailabilityDto>> GetDriverAvailabilityAsync(int userId)
        {
            var availabilities = await _driverAvailabilityRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<DriverAvailabilityDto>>(availabilities);
        }

        public async Task<bool> SaveDriverAvailabilityAsync(int userId, DriverAvailabilityBulkDto bulkDto)
        {
            // Validate user exists and is a driver
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.RoleId != 4)
                throw new ArgumentException("User not found or not a driver");

            // Delete existing availabilities
            await _driverAvailabilityRepository.DeleteByUserIdAsync(userId);

            // Create new availabilities for selected days
            var availabilities = new List<DriverAvailability>();
            foreach (var day in bulkDto.Days)
            {
                if (day.Value) // If day is selected
                {
                    availabilities.Add(new DriverAvailability
                    {
                        UserId = userId,
                        DayOfWeek = day.Key,
                        StartTime = bulkDto.StartTime,
                        EndTime = bulkDto.EndTime,
                        IsActive = true
                    });
                }
            }

            if (availabilities.Any())
            {
                await _driverAvailabilityRepository.AddRangeAsync(availabilities);
            }

            return true;
        }

        public async Task<bool> DeleteDriverAvailabilityAsync(int userId, int availabilityId)
        {
            // Validate user exists and is a driver
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.RoleId != 3)
                throw new ArgumentException("User not found or not a driver");

            // Get the availability to delete
            var availability = await _driverAvailabilityRepository.GetByIdAsync(availabilityId);
            if (availability == null || availability.UserId != userId)
                return false;

            // Delete the availability
            await _driverAvailabilityRepository.DeleteAsync(availability);
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetDriversAssignedToVansAsync()
        {
            var users = await _userRepository.GetDriversAssignedToVansAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<string> UploadUserImageAsync(int userId, IFormFile image)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Delete old image if exists
            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                await _fileUploadService.DeleteImageAsync(user.ImageUrl);
            }

            // Upload new image
            var imageUrl = await _fileUploadService.UploadImageAsync(image, "users");

            // Update user with new image URL
            user.ImageUrl = imageUrl;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return imageUrl;
        }
    }
}
