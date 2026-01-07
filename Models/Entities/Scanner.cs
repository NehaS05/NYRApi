using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class Scanner
    {
        [Key]
        public int Id { get; set; }

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

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Refresh Token fields for scanner sessions
        [MaxLength(500)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}

