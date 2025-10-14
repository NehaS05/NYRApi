using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDriverAvailabilityService _driverAvailabilityService;

        public UsersController(IUserService userService, IDriverAvailabilityService driverAvailabilityService)
        {
            _userService = userService;
            _driverAvailabilityService = driverAvailabilityService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("by-role/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(int roleId)
        {
            var users = await _userService.GetUsersByRoleAsync(roleId);
            return Ok(users);
        }

        [HttpGet("by-customer/{customerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByCustomer(int customerId)
        {
            var users = await _userService.GetUsersByCustomerAsync(customerId);
            return Ok(users);
        }

        [HttpGet("by-location/{locationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByLocation(int locationId)
        {
            var users = await _userService.GetUsersByLocationAsync(locationId);
            return Ok(users);
        }

        [HttpGet("{id}/driver-availability")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DriverAvailabilityDto>>> GetDriverAvailability(int id)
        {
            var availabilities = await _userService.GetDriverAvailabilityAsync(id);
            return Ok(availabilities);
        }

        [HttpPost("{id}/driver-availability")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SaveDriverAvailability(int id, [FromBody] DriverAvailabilityBulkDto bulkDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.SaveDriverAvailabilityAsync(id, bulkDto);
                if (result)
                    return Ok(new { message = "Driver availability saved successfully" });
                else
                    return BadRequest("Failed to save driver availability");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/driver-availability/individual")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateIndividualDriverAvailability(int id, [FromBody] CreateDriverAvailabilityDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Ensure the userId in the DTO matches the route parameter
                createDto.UserId = id;
                
                var result = await _driverAvailabilityService.CreateAsync(createDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}/driver-availability/{availabilityId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDriverAvailability(int id, int availabilityId)
        {
            try
            {
                var result = await _userService.DeleteDriverAvailabilityAsync(id, availabilityId);
                if (result)
                    return Ok(new { message = "Driver availability deleted successfully" });
                else
                    return NotFound("Driver availability not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
