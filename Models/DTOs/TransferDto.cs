using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class TransferDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // "VanTransfer" or "RestockRequest"
        public int? LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime? DeliveryDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string? DriverName { get; set; }
        public int? DriverId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TransferSummaryDto
    {
        public int TotalVanTransfers { get; set; }
        public int TotalRestockRequests { get; set; }
        public int TotalTransfers { get; set; }
        public int PendingTransfers { get; set; }
        public int CompletedTransfers { get; set; }
    }
}
