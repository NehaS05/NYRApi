using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? BarcodeSKU { get; set; }
        public string? BarcodeSKU2 { get; set; }
        public string? BarcodeSKU3 { get; set; }
        public string? BarcodeSKU4 { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool ShowInCatalogue { get; set; }
        public bool IsUniversal { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Product variants - unique combinations of variations
        /// </summary>
        public List<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
    }

    public class CreateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU2 { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU3 { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU4 { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public bool ShowInCatalogue { get; set; } = true;

        public bool IsUniversal { get; set; } = false;
        
        /// <summary>
        /// Product variants to create - unique combinations of variations
        /// </summary>
        public List<CreateProductVariantDto> Variants { get; set; } = new List<CreateProductVariantDto>();
    }

    public class UpdateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU2 { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU3 { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU4 { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public bool ShowInCatalogue { get; set; } = true;

        public bool IsUniversal { get; set; } = false;

        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Product variants to update - unique combinations of variations
        /// </summary>
        public List<CreateProductVariantDto> Variants { get; set; } = new List<CreateProductVariantDto>();
    }
}
