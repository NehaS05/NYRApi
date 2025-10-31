using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class ScannerDto
    {
        public int Id { get; set; }
        public string ScannerId { get; set; } = string.Empty;
        public string ScannerName { get; set; } = string.Empty;
        public string ScannerPIN { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateScannerDto
    {
        [Required]
        [MaxLength(100)]
        public string ScannerId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ScannerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ScannerPIN { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }
    }

    public class UpdateScannerDto
    {
        [Required]
        [MaxLength(100)]
        public string ScannerId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ScannerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ScannerPIN { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

