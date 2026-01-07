using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string? BarcodeSKU { get; set; }
        public string? BarcodeSKU2 { get; set; }
        public string? BarcodeSKU3 { get; set; }
        public string? BarcodeSKU4 { get; set; }
        public string? SKU { get; set; }
        public bool IsEnabled { get; set; }
        public List<ProductVariantAttributeDto> Attributes { get; set; } = new List<ProductVariantAttributeDto>();
    }

    public class ProductVariantAttributeDto
    {
        public int Id { get; set; }
        public int VariationId { get; set; }
        public string VariationName { get; set; } = string.Empty;
        public int VariationOptionId { get; set; }
        public string VariationOptionName { get; set; } = string.Empty;
        public string? VariationOptionValue { get; set; }
    }

    public class CreateProductVariantDto
    {
        [Required]
        public string VariantName { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [MaxLength(255)]
        public string? ImageUrl { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [MaxLength(100)]
        public string? BarcodeSKU { get; set; }
        
        [MaxLength(100)]
        public string? BarcodeSKU2 { get; set; }
        
        [MaxLength(100)]
        public string? BarcodeSKU3 { get; set; }
        
        [MaxLength(100)]
        public string? BarcodeSKU4 { get; set; }
        
        [MaxLength(100)]
        public string? SKU { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        public List<CreateProductVariantAttributeDto> Attributes { get; set; } = new List<CreateProductVariantAttributeDto>();
    }

    public class CreateProductVariantAttributeDto
    {
        public int VariationId { get; set; }
        public int VariationOptionId { get; set; }
    }
}
