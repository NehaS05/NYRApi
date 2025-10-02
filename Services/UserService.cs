using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ICustomerRepository customerRepository,
            ILocationRepository locationRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _customerRepository = customerRepository;
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(createUserDto.RoleId);
            if (role == null)
                throw new ArgumentException("Invalid role ID");

            // Validate customer exists if provided
            if (createUserDto.CustomerId.HasValue)
            {
                var customer = await _customerRepository.GetByIdAsync(createUserDto.CustomerId.Value);
                if (customer == null)
                    throw new ArgumentException("Invalid customer ID");
            }

            // Validate location exists if provided
            if (createUserDto.LocationId.HasValue)
            {
                var location = await _locationRepository.GetByIdAsync(createUserDto.LocationId.Value);
                if (location == null)
                    throw new ArgumentException("Invalid location ID");
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
    }
}
