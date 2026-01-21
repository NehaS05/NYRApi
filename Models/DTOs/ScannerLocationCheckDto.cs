namespace NYR.API.Models.DTOs
{
    public class ScannerLocationCheckDto
    {
        public bool IsLocation { get; set; }
        public string SerialNo { get; set; } = string.Empty;
        public SimpleLocationDto? AssignedLocation { get; set; }
        public IEnumerable<SimpleLocationDto> AvailableLocations { get; set; } = new List<SimpleLocationDto>();
        public string Message { get; set; } = string.Empty;
        
        // Token fields for valid scanners
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }

    public class SimpleLocationDto
    {
        public int Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }
}