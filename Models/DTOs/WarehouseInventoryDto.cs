using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class WarehouseInventoryDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseAddress { get; set; } = string.Empty;
        public string WarehouseCity { get; set; } = string.Empty;
        public string WarehouseState { get; set; } = string.Empty;
        public string WarehouseZipCode { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public int ProductVariationId { get; set; }
        public string VariationType { get; set; } = string.Empty;
        public string VariationValue { get; set; } = string.Empty;
        public string? VariationSKU { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class AddInventoryDto
    {
        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductVariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class AddBulkInventoryDto
    {
        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one inventory item is required")]
        public List<BulkInventoryItemDto> InventoryItems { get; set; } = new List<BulkInventoryItemDto>();
    }

    public class BulkInventoryItemDto
    {
        [Required]
        public int ProductVariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class WarehouseListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class WarehouseInventoryDetailDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public string VariationType { get; set; } = string.Empty;
        public string VariationValue { get; set; } = string.Empty;
        public string? VariationSKU { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateInventoryDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
