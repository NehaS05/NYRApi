using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class RestockRequestDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<RestockRequestItemDto> Items { get; set; } = new List<RestockRequestItemDto>();
    }

    public class RestockRequestItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SkuCode { get; set; }
        public int? ProductVariantId { get; set; }
        public string? VariantName { get; set; }
        public string? VariationType { get; set; }
        public string? VariationValue { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateRestockRequestDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateRestockRequestItemDto> Items { get; set; } = new List<CreateRestockRequestItemDto>();
    }

    public class CreateRestockRequestItemDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? ProductVariantId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class RestockRequestSummaryDto
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? LocationNumber { get; set; }
        public int TotalRequests { get; set; }
        public int TotalItems { get; set; }
    }
}
