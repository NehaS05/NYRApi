using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class ProductVariationService : IProductVariationService
    {
        private readonly IProductVariationRepository _variationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductVariationService(
            IProductVariationRepository variationRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _variationRepository = variationRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVariationDto>> GetAllVariationsAsync()
        {
            var variations = await _variationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductVariationDto>>(variations);
        }

        public async Task<ProductVariationDto?> GetVariationByIdAsync(int id)
        {
            var variation = await _variationRepository.GetByIdAsync(id);
            return variation != null ? _mapper.Map<ProductVariationDto>(variation) : null;
        }

        public async Task<ProductVariationDto> CreateVariationAsync(CreateProductVariationDto createVariationDto)
        {
            // Validate product exists
            var product = await _productRepository.GetByIdAsync(createVariationDto.ProductId);
            if (product == null)
                throw new ArgumentException("Invalid product ID");

            var variation = _mapper.Map<ProductVariation>(createVariationDto);
            var createdVariation = await _variationRepository.AddAsync(variation);
            return _mapper.Map<ProductVariationDto>(createdVariation);
        }

        public async Task<ProductVariationDto?> UpdateVariationAsync(int id, UpdateProductVariationDto updateVariationDto)
        {
            var variation = await _variationRepository.GetByIdAsync(id);
            if (variation == null)
                return null;

            _mapper.Map(updateVariationDto, variation);
            variation.UpdatedAt = DateTime.UtcNow;

            await _variationRepository.UpdateAsync(variation);
            return _mapper.Map<ProductVariationDto>(variation);
        }

        public async Task<bool> DeleteVariationAsync(int id)
        {
            var variation = await _variationRepository.GetByIdAsync(id);
            if (variation == null)
                return false;

            await _variationRepository.DeleteAsync(variation);
            return true;
        }

        public async Task<IEnumerable<ProductVariationDto>> GetVariationsByProductAsync(int productId)
        {
            var variations = await _variationRepository.GetVariationsByProductAsync(productId);
            return _mapper.Map<IEnumerable<ProductVariationDto>>(variations);
        }

        public async Task<IEnumerable<ProductVariationDto>> GetVariationsByTypeAsync(int productId, string variationType)
        {
            var variations = await _variationRepository.GetVariationsByTypeAsync(productId, variationType);
            return _mapper.Map<IEnumerable<ProductVariationDto>>(variations);
        }
    }
}
