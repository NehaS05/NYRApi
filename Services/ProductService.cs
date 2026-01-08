using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            ISupplierRepository supplierRepository,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _supplierRepository = supplierRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            products = products.Where(x => x.IsActive == true);
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

            // Validate that variants are provided and each has a price
            if (createProductDto.Variants == null || !createProductDto.Variants.Any())
                throw new ArgumentException("At least one variant is required");

            var product = _mapper.Map<Product>(createProductDto);
            var createdProduct = await _productRepository.AddAsync(product);

            // Create variants with migrated fields
            foreach (var variantDto in createProductDto.Variants)
            {
                // Validate that price is provided for each variant
                if (variantDto.Price <= 0)
                    throw new ArgumentException($"Price is required and must be greater than 0 for variant: {variantDto.VariantName}");

                var variant = new ProductVariant
                {
                    ProductId = createdProduct.Id,
                    VariantName = variantDto.VariantName,
                    Description = variantDto.Description,
                    ImageUrl = variantDto.ImageUrl,
                    Price = variantDto.Price,
                    BarcodeSKU = variantDto.BarcodeSKU,
                    BarcodeSKU2 = variantDto.BarcodeSKU2,
                    BarcodeSKU3 = variantDto.BarcodeSKU3,
                    BarcodeSKU4 = variantDto.BarcodeSKU4,
                    SKU = variantDto.SKU,
                    IsEnabled = variantDto.IsEnabled,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Add variant attributes
                foreach (var attrDto in variantDto.Attributes)
                {
                    variant.Attributes.Add(new ProductVariantAttribute
                    {
                        VariationId = attrDto.VariationId,
                        VariationOptionId = attrDto.VariationOptionId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                createdProduct.Variants.Add(variant);
            }
            
            await _productRepository.UpdateAsync(createdProduct);

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

            // Validate that variants are provided and each has a price
            if (updateProductDto.Variants == null || !updateProductDto.Variants.Any())
                throw new ArgumentException("At least one variant is required");

            _mapper.Map(updateProductDto, product);
            product.UpdatedAt = DateTime.UtcNow;

            // Update variants - preserve existing variants and update their properties
            var existingVariants = await _context.ProductVariants
                .Include(pv => pv.Attributes)
                .Where(pv => pv.ProductId == id)
                .ToListAsync();

            // Create a dictionary of existing variants by their ID for quick lookup
            var existingVariantsDict = existingVariants.ToDictionary(v => v.Id);

            // Process incoming variants
            foreach (var variantDto in updateProductDto.Variants)
            {
                // Validate that price is provided for each variant
                if (variantDto.Price <= 0)
                    throw new ArgumentException($"Price is required and must be greater than 0 for variant: {variantDto.VariantName}");

                ProductVariant variant;
                
                if (variantDto.Id.HasValue && existingVariantsDict.ContainsKey(variantDto.Id.Value))
                {
                    // Update existing variant
                    variant = existingVariantsDict[variantDto.Id.Value];
                    variant.VariantName = variantDto.VariantName;
                    variant.Description = variantDto.Description;
                    variant.ImageUrl = variantDto.ImageUrl;
                    variant.Price = variantDto.Price;
                    variant.BarcodeSKU = variantDto.BarcodeSKU;
                    variant.BarcodeSKU2 = variantDto.BarcodeSKU2;
                    variant.BarcodeSKU3 = variantDto.BarcodeSKU3;
                    variant.BarcodeSKU4 = variantDto.BarcodeSKU4;
                    variant.SKU = variantDto.SKU;
                    variant.IsEnabled = variantDto.IsEnabled;
                    variant.UpdatedAt = DateTime.UtcNow;

                    // Update attributes - remove existing and add new ones
                    _context.ProductVariantAttributes.RemoveRange(variant.Attributes);
                    variant.Attributes.Clear();
                    
                    // Remove from dictionary so we know it was processed
                    existingVariantsDict.Remove(variantDto.Id.Value);
                }
                else
                {
                    // Create new variant
                    variant = new ProductVariant
                    {
                        ProductId = product.Id,
                        VariantName = variantDto.VariantName,
                        Description = variantDto.Description,
                        ImageUrl = variantDto.ImageUrl,
                        Price = variantDto.Price,
                        BarcodeSKU = variantDto.BarcodeSKU,
                        BarcodeSKU2 = variantDto.BarcodeSKU2,
                        BarcodeSKU3 = variantDto.BarcodeSKU3,
                        BarcodeSKU4 = variantDto.BarcodeSKU4,
                        SKU = variantDto.SKU,
                        IsEnabled = variantDto.IsEnabled,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    
                    product.Variants.Add(variant);
                }

                // Add variant attributes
                foreach (var attrDto in variantDto.Attributes)
                {
                    variant.Attributes.Add(new ProductVariantAttribute
                    {
                        VariationId = attrDto.VariationId,
                        VariationOptionId = attrDto.VariationOptionId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Handle variants that were not in the update request - soft delete them
            foreach (var remainingVariant in existingVariantsDict.Values)
            {
                // Check if this variant is referenced by other entities
                var hasReferences = await _context.RestockRequestItems.AnyAsync(rri => rri.ProductVariantId == remainingVariant.Id) ||
                                   await _context.LocationInventoryData.AnyAsync(lid => lid.ProductVariantId == remainingVariant.Id) ||
                                   await _context.LocationOutwardInventories.AnyAsync(loi => loi.ProductVariantId == remainingVariant.Id);

                if (hasReferences)
                {
                    // Soft delete - mark as inactive
                    remainingVariant.IsActive = false;
                    remainingVariant.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Hard delete - no references found
                    _context.ProductVariantAttributes.RemoveRange(remainingVariant.Attributes);
                    _context.ProductVariants.Remove(remainingVariant);
                }
            }

            await _productRepository.UpdateAsync(product);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            // Check if there are product variants associated with this product
            var hasVariantsQuery = _context.ProductVariants
                .Where(pv => pv.ProductId == id && pv.IsActive);
            
            var hasVariants = await hasVariantsQuery.AnyAsync();
            
            if (hasVariants)
            {
                // Check if any variants are referenced in LocationInventoryData or RequestSupplies
                var hasInventoryReferencesQuery = _context.LocationInventoryData
                    .Where(lid => lid.ProductId == id || 
                                 (lid.ProductVariantId.HasValue && 
                                  _context.ProductVariants.Any(pv => pv.Id == lid.ProductVariantId.Value && pv.ProductId == id)));
                
                var hasInventoryReferences = await hasInventoryReferencesQuery.AnyAsync();
                
                // Check if product is referenced in RequestSupplies
                var hasRequestSuppliesReferences = await _context.RequestSupplies
                    .AnyAsync(rs => rs.ProductId == id);
                
                if (hasInventoryReferences || hasRequestSuppliesReferences)
                {
                    // Soft delete: deactivate product and its variants
                    product.IsActive = false;
                    product.UpdatedAt = DateTime.UtcNow;
                    await _productRepository.UpdateAsync(product);
                    
                    // Deactivate associated product variants
                    var variants = await hasVariantsQuery.ToListAsync();
                    foreach (var variant in variants)
                    {
                        variant.IsActive = false;
                        variant.UpdatedAt = DateTime.UtcNow;
                    }
                    
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // No inventory or request supplies references, but has variants - soft delete product and variants
                    product.IsActive = false;
                    product.UpdatedAt = DateTime.UtcNow;
                    await _productRepository.UpdateAsync(product);
                    
                    var variants = await hasVariantsQuery.ToListAsync();
                    foreach (var variant in variants)
                    {
                        variant.IsActive = false;
                        variant.UpdatedAt = DateTime.UtcNow;
                    }
                    
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Check if product itself is referenced in LocationInventoryData or RequestSupplies
                var hasDirectInventoryReferences = await _context.LocationInventoryData
                    .AnyAsync(lid => lid.ProductId == id);
                
                var hasDirectRequestSuppliesReferences = await _context.RequestSupplies
                    .AnyAsync(rs => rs.ProductId == id);
                
                if (hasDirectInventoryReferences || hasDirectRequestSuppliesReferences)
                {
                    // Soft delete product
                    product.IsActive = false;
                    product.UpdatedAt = DateTime.UtcNow;
                    await _productRepository.UpdateAsync(product);
                }
                else
                {
                    // No references, safe to hard delete
                    await _productRepository.DeleteAsync(product);
                }
            }
            
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
