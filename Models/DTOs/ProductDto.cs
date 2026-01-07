using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
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

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int SupplierId { get; set; }

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

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        public bool ShowInCatalogue { get; set; } = true;

        public bool IsUniversal { get; set; } = false;

        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Product variants to update - unique combinations of variations
        /// </summary>
        public List<CreateProductVariantDto> Variants { get; set; } = new List<CreateProductVariantDto>();
    }
}
