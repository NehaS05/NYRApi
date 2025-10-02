using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role != null ? _mapper.Map<RoleDto>(role) : null;
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            // Check if role name already exists
            var existingRole = await _roleRepository.GetByNameAsync(createRoleDto.Name);
            if (existingRole != null)
                throw new ArgumentException("Role name already exists");

            var role = _mapper.Map<Role>(createRoleDto);
            var createdRole = await _roleRepository.AddAsync(role);
            return _mapper.Map<RoleDto>(createdRole);
        }

        public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return null;

            // Check if role name already exists for another role
            var existingRole = await _roleRepository.GetByNameAsync(updateRoleDto.Name);
            if (existingRole != null && existingRole.Id != id)
                throw new ArgumentException("Role name already exists");

            _mapper.Map(updateRoleDto, role);
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateAsync(role);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return false;

            await _roleRepository.DeleteAsync(role);
            return true;
        }

        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            var roles = await _roleRepository.GetActiveRolesAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
    }
}
