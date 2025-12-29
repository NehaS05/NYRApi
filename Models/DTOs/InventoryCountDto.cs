using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class InventoryCountsByDriverDto
    {
        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public List<WarehouseInventoryCountDto> WarehouseInventories { get; set; } = new List<WarehouseInventoryCountDto>();
        public List<VanInventoryCountDto> VanInventories { get; set; } = new List<VanInventoryCountDto>();
        public int TotalWarehouseItems { get; set; }
        public int TotalVanItems { get; set; }
        public int TotalItems { get; set; }
    }

    public class WarehouseInventoryCountDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int? ProductVariantId { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class VanInventoryCountDto
    {
        public int Id { get; set; }
        public int VanId { get; set; }
        public string VanName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int? ProductVariantId { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}