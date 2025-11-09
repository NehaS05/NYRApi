using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class ProductVariationDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VariationType { get; set; } = string.Empty;
        public string VariationValue { get; set; } = string.Empty;
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
    }

    public class UpdateProductVariationDto
    {
        [Required]
        [MaxLength(50)]
        public string VariationType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string VariationValue { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
