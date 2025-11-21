using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class RouteStop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RouteId { get; set; }

        [ForeignKey("RouteId")]
        public virtual Routes Route { get; set; } = null!;

        [Required]
        public int StopOrder { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? CustomerName { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Skipped

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
