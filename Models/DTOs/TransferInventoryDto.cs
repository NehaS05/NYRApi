using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class TransferInventoryDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? LocationNumber { get; set; }
        public DateTime TransferDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<TransferInventoryItemDto> Items { get; set; } = new List<TransferInventoryItemDto>();
    }

    public class TransferInventoryItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int? ProductVariationId { get; set; }
        public string? VariationType { get; set; }
        public string? VariationValue { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateTransferInventoryDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public List<CreateTransferInventoryItemDto> Items { get; set; } = new List<CreateTransferInventoryItemDto>();
    }

    public class CreateTransferInventoryItemDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? ProductVariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class TransferInventoryLocationDto
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? LocationNumber { get; set; }
    }
}
