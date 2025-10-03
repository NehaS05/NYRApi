using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariationRepository _variationRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            IProductVariationRepository variationRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            ISupplierRepository supplierRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _variationRepository = variationRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductWithDetailsAsync(id);
            return product != null ? _mapper.Map<ProductDto>(product) : null;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(createProductDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Invalid category ID");

            // Validate brand exists
            var brand = await _brandRepository.GetByIdAsync(createProductDto.BrandId);
            if (brand == null)
                throw new ArgumentException("Invalid brand ID");

            // Validate supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(createProductDto.SupplierId);
            if (supplier == null)
                throw new ArgumentException("Invalid supplier ID");

            var product = _mapper.Map<Product>(createProductDto);
            var createdProduct = await _productRepository.AddAsync(product);

            // Create variations if any
            if (createProductDto.Variations.Any())
            {
                foreach (var variationDto in createProductDto.Variations)
                {
                    var variation = _mapper.Map<ProductVariation>(variationDto);
                    variation.ProductId = createdProduct.Id;
                    await _variationRepository.AddAsync(variation);
                }
            }

            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Invalid category ID");

            // Validate brand exists
            var brand = await _brandRepository.GetByIdAsync(updateProductDto.BrandId);
            if (brand == null)
                throw new ArgumentException("Invalid brand ID");

            // Validate supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(updateProductDto.SupplierId);
            if (supplier == null)
                throw new ArgumentException("Invalid supplier ID");

            _mapper.Map(updateProductDto, product);
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            // Update variations
            if (updateProductDto.Variations.Any())
            {
                // Remove existing variations
                var existingVariations = await _variationRepository.GetVariationsByProductAsync(id);
                foreach (var variation in existingVariations)
                {
                    await _variationRepository.DeleteAsync(variation);
                }

                // Add new variations
                foreach (var variationDto in updateProductDto.Variations)
                {
                    var variation = _mapper.Map<ProductVariation>(variationDto);
                    variation.ProductId = id;
                    await _variationRepository.AddAsync(variation);
                }
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            // Delete associated variations first
            var variations = await _variationRepository.GetVariationsByProductAsync(id);
            foreach (var variation in variations)
            {
                await _variationRepository.DeleteAsync(variation);
            }

            await _productRepository.DeleteAsync(product);
            return true;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByBrandAsync(int brandId)
        {
            var products = await _productRepository.GetProductsByBrandAsync(brandId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsBySupplierAsync(int supplierId)
        {
            var products = await _productRepository.GetProductsBySupplierAsync(supplierId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetCatalogueProductsAsync()
        {
            var products = await _productRepository.GetCatalogueProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetUniversalProductsAsync()
        {
            var products = await _productRepository.GetUniversalProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByBarcodeAsync(string barcode)
        {
            var product = await _productRepository.GetByBarcodeAsync(barcode);
            return product != null ? _mapper.Map<ProductDto>(product) : null;
        }
    }
}
