using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BrandDto>>(brands);
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand != null ? _mapper.Map<BrandDto>(brand) : null;
        }

        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            // Check if brand with same name already exists
            var existingBrand = await _brandRepository.GetByNameAsync(createBrandDto.Name);
            if (existingBrand != null)
                throw new ArgumentException("Brand with this name already exists");

            var brand = _mapper.Map<Brand>(createBrandDto);
            var createdBrand = await _brandRepository.AddAsync(brand);
            return _mapper.Map<BrandDto>(createdBrand);
        }

        public async Task<BrandDto?> UpdateBrandAsync(int id, UpdateBrandDto updateBrandDto)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return null;

            // Check if another brand with same name exists
            var existingBrand = await _brandRepository.GetByNameAsync(updateBrandDto.Name);
            if (existingBrand != null && existingBrand.Id != id)
                throw new ArgumentException("Brand with this name already exists");

            _mapper.Map(updateBrandDto, brand);
            brand.UpdatedAt = DateTime.UtcNow;

            await _brandRepository.UpdateAsync(brand);
            return _mapper.Map<BrandDto>(brand);
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return false;

            await _brandRepository.DeleteAsync(brand);
            return true;
        }

        public async Task<IEnumerable<BrandDto>> GetActiveBrandsAsync()
        {
            var brands = await _brandRepository.GetActiveBrandsAsync();
            return _mapper.Map<IEnumerable<BrandDto>>(brands);
        }
    }
}
