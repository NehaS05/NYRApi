using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class ProductVariationDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VariationType { get; set; } = string.Empty;
        public string VariationValue { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public decimal? PriceAdjustment { get; set; }
        public int? StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductVariationDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string VariationType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string VariationValue { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SKU { get; set; }

        public decimal? PriceAdjustment { get; set; } = 0;

        public int? StockQuantity { get; set; }
    }

    public class UpdateProductVariationDto
    {
        [Required]
        [MaxLength(50)]
        public string VariationType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string VariationValue { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SKU { get; set; }

        public decimal? PriceAdjustment { get; set; } = 0;

        public int? StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
