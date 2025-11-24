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
        public int? LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; } = string.Empty;
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
        public int WarehouseId { get; set; }

        // Optional for future phase - delivery location
        public int? LocationId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [MaxLength(100)]
        public string? DriverName { get; set; }

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

    public class TransferTrackingDto
    {
        public int Id { get; set; }
        public int VanId { get; set; }
        public string VanName { get; set; } = string.Empty;
        public string VanNumber { get; set; } = string.Empty;
        public int? LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DriverName { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateTransferStatusDto
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public DateTime? DeliveryDate { get; set; }

        [MaxLength(100)]
        public string? DriverName { get; set; }
    }
