using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class ScannerDto
    {
        public int Id { get; set; }
        public string SerialNo { get; set; } = string.Empty;
        public string ScannerName { get; set; } = string.Empty;
        public string ScannerPIN { get; set; } = string.Empty;
        public string? ScannerUrl { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool AppPinReset { get; set; }
    }

    public class CreateScannerDto
    {
        [Required]
        [MaxLength(100)]
        public string SerialNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ScannerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ScannerPIN { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ScannerUrl { get; set; }

        [Required]
        public int LocationId { get; set; }

        public bool AppPinReset { get; set; } = false;
    }

    public class UpdateScannerDto
    {
        [Required]
        [MaxLength(100)]
        public string SerialNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ScannerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ScannerPIN { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ScannerUrl { get; set; }

        [Required]
        public int LocationId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool AppPinReset { get; set; } = false;
    }

    public class ScannerPinConfirmDto
    {
        [Required]
        [MaxLength(100)]
        public string SerialNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ScannerPIN { get; set; } = string.Empty;
    }

    public class ScannerPinConfirmResponseDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool AppPinReset { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }

    public class ScannerPinResetDto
    {
        [Required]
        [MaxLength(100)]
        public string SerialNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NewPIN { get; set; } = string.Empty;
    }

    public class ScannerPinResetResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class SetAppPinResetDto
    {
        [Required]
        [MaxLength(100)]
        public string SerialNo { get; set; } = string.Empty;

        [Required]
        public bool AppPinReset { get; set; }
    }
}

