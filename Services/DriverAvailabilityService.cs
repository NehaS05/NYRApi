using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class DriverAvailabilityService : IDriverAvailabilityService
    {
        private readonly IDriverAvailabilityRepository _driverAvailabilityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DriverAvailabilityService(
            IDriverAvailabilityRepository driverAvailabilityRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _driverAvailabilityRepository = driverAvailabilityRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DriverAvailabilityDto>> GetByUserIdAsync(int userId)
        {
            var availabilities = await _driverAvailabilityRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<DriverAvailabilityDto>>(availabilities);
        }

        public async Task<DriverAvailabilityDto> CreateAsync(CreateDriverAvailabilityDto createDto)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(createDto.UserId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Check if availability already exists for this user and day
            var existingAvailability = await _driverAvailabilityRepository.GetByUserIdAndDayAsync(createDto.UserId, createDto.DayOfWeek);
            if (existingAvailability != null)
                throw new ArgumentException($"Availability already exists for {createDto.DayOfWeek}");

            var availability = _mapper.Map<DriverAvailability>(createDto);
            var createdAvailability = await _driverAvailabilityRepository.AddAsync(availability);
            return _mapper.Map<DriverAvailabilityDto>(createdAvailability);
        }

        public async Task<DriverAvailabilityDto?> UpdateAsync(int id, UpdateDriverAvailabilityDto updateDto)
        {
            var availability = await _driverAvailabilityRepository.GetByIdAsync(id);
            if (availability == null)
                return null;

            _mapper.Map(updateDto, availability);
            availability.UpdatedAt = DateTime.UtcNow;

            await _driverAvailabilityRepository.UpdateAsync(availability);
            return _mapper.Map<DriverAvailabilityDto>(availability);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var availability = await _driverAvailabilityRepository.GetByIdAsync(id);
            if (availability == null)
                return false;

            await _driverAvailabilityRepository.DeleteAsync(availability);
            return true;
        }

        public async Task<bool> DeleteByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            await _driverAvailabilityRepository.DeleteByUserIdAsync(userId);
            return true;
        }

        public async Task<bool> SaveBulkAsync(DriverAvailabilityBulkDto bulkDto)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(bulkDto.UserId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Delete existing availabilities for this user
            await _driverAvailabilityRepository.DeleteByUserIdAsync(bulkDto.UserId);

            // Create new availabilities for selected days
            var availabilities = new List<DriverAvailability>();
            foreach (var day in bulkDto.Days)
            {
                if (day.Value) // If day is selected
                {
                    availabilities.Add(new DriverAvailability
                    {
                        UserId = bulkDto.UserId,
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

        public async Task<IEnumerable<DriverAvailabilityDto>> GetActiveByUserIdAsync(int userId)
        {
            var availabilities = await _driverAvailabilityRepository.GetActiveByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<DriverAvailabilityDto>>(availabilities);
        }
    }
}
