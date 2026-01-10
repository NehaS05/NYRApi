using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class LocationUnlistedInventoryDto
    {
        public int Id { get; set; }
        public string BarcodeNo { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateLocationUnlistedInventoryDto
    {
        [Required]
        [MaxLength(100)]
        public string BarcodeNo { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public int CreatedBy { get; set; }
    }

    public class UpdateLocationUnlistedInventoryDto
    {
        [Required]
        [MaxLength(100)]
        public string BarcodeNo { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public int UpdatedBy { get; set; }
    }
}