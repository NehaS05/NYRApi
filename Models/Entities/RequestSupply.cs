using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class RequestSupply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;

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
