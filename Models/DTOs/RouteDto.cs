using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NYR.API.Models.DTOs
{
    public class RouteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;           //It's fro Starting Point
        public int WarehouseId { get; set; }           //It's fro Starting Point

        [JsonConverter(typeof(NYR.API.Helpers.DateOnlyJsonConverter))]
        public DateTime DeliveryDate { get; set; }
        
        public string Status { get; set; } = "Draft";
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<RouteStopDto> RouteStops { get; set; } = new List<RouteStopDto>();
    }

    public class RouteStopDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int StopOrder { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? RestockRequestId { get; set; }
        public int? FollowupRequestId { get; set; }
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ContactPhone { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Draft";
        public string? DeliveryOTP { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<TransferInventoryItemDto> ShippingInventory { get; set; } = new List<TransferInventoryItemDto>();
        public List<LocationInventoryDataDto> LocationInventory { get; set; } = new List<LocationInventoryDataDto>();
    }

    public class CreateRouteDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        public List<CreateRouteStopDto> RouteStops { get; set; } = new List<CreateRouteStopDto>();
    }

    public class CreateRouteStopDto
    {
        [Required]
        public int LocationId { get; set; }

        [Required]
        public int StopOrder { get; set; }

        public int? CustomerId { get; set; }

        public int? RestockRequestId { get; set; }

        public int? FollowupRequestId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateRouteDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        public bool IsActive { get; set; } = true;

        public List<UpdateRouteStopDto>? RouteStops { get; set; } = new List<UpdateRouteStopDto>();
    }

    public class UpdateRouteStopDto
    {
        public int? Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public int StopOrder { get; set; }

        public int? CustomerId { get; set; }

        public int? RestockRequestId { get; set; }

        public int? FollowupRequestId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        [MaxLength(10)]
        public string? DeliveryOTP { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
