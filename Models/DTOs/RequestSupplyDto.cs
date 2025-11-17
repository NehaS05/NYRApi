using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class RequestSupplyDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SuppliersName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string? EmailTemplate { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<RequestSupplyItemDto> Items { get; set; } = new List<RequestSupplyItemDto>();
    }

    public class RequestSupplyItemDto
    {
        public int Id { get; set; }
        public int RequestSupplyId { get; set; }
        public int VariationId { get; set; }
        public string VariationName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateRequestSupplyDto
    {
        //[Required]
        //[MaxLength(200)]
        //public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string EmailAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? EmailTemplate { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public List<CreateRequestSupplyItemDto> Items { get; set; } = new List<CreateRequestSupplyItemDto>();
    }

    public class CreateRequestSupplyItemDto
    {
        [Required]
        public int VariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class UpdateRequestSupplyDto
    {
        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string EmailAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? EmailTemplate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public bool IsActive { get; set; } = true;

        public List<UpdateRequestSupplyItemDto> Items { get; set; } = new List<UpdateRequestSupplyItemDto>();
    }

    public class UpdateRequestSupplyItemDto
    {
        public int? Id { get; set; }

        [Required]
        public int VariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
