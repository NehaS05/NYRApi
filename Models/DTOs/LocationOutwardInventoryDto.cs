using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class LocationOutwardInventoryDto
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ProductVariantId { get; set; }
        public string? VariationName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateLocationOutwardInventoryDto
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public int? ProductVariantId { get; set; }

        [MaxLength(200)]
        public string? VariationName { get; set; }
    }

    public class UpdateLocationOutwardInventoryDto
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public int UpdatedBy { get; set; }

        public int? ProductVariantId { get; set; }

        [MaxLength(200)]
        public string? VariationName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}