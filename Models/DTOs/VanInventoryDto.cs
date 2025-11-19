using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class VanInventoryDto
    {
        public int Id { get; set; }
        public int VanId { get; set; }
        public string VanName { get; set; } = string.Empty;
        public string VanNumber { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public List<VanInventoryItemDto> Items { get; set; } = new List<VanInventoryItemDto>();
    }

    public class VanInventoryItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int ProductVariationId { get; set; }
        public string? VariationType { get; set; }
        public string? VariationValue { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateVanInventoryDto
    {
        [Required]
        public int VanId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateVanInventoryItemDto> Items { get; set; } = new List<CreateVanInventoryItemDto>();
    }

    public class CreateVanInventoryItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductVariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class VanWithInventorySummaryDto
    {
        public int VanId { get; set; }
        public string VanName { get; set; } = string.Empty;
        public string VanNumber { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public int TotalTransfers { get; set; }
        public int TotalItems { get; set; }
    }
}
