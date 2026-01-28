namespace NYR.API.Models.DTOs
{
    public class LocationInventoryGroupDto
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? LocationNumber { get; set; }
        public IEnumerable<LocationInventoryDataDto> InventoryItems { get; set; } = new List<LocationInventoryDataDto>();
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}