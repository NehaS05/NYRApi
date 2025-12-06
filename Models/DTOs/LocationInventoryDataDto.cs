using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class LocationInventoryDataDto
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? VariationName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateLocationInventoryDataDto
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; }

        [MaxLength(200)]
        public string? VariationName { get; set; }

        [Required]
        public int CreatedBy { get; set; }
    }

    public class UpdateLocationInventoryDataDto
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; }

        [MaxLength(200)]
        public string? VariationName { get; set; }

        [Required]
        public int UpdatedBy { get; set; }
    }
}
