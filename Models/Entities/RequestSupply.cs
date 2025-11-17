using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class RequestSupply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(500)]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(500)]
        public string EmailAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? EmailTemplate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        // Navigation properties
        public virtual ICollection<RequestSupplyItem> Items { get; set; } = new List<RequestSupplyItem>();
    }
}
