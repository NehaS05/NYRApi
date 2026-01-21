using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class AssignScannerToLocationDto
    {
        [Required]
        [MaxLength(100)]
        public string SerialNo { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        [Required]
        [MaxLength(10)]
        public string AdminPIN { get; set; } = string.Empty;
    }

    public class AssignScannerToLocationResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }
}