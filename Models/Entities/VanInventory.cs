using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class VanInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VanId { get; set; }

        [ForeignKey("VanId")]
        public virtual Van Van { get; set; } = null!;

        // Optional for future phase - delivery location
        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location? Location { get; set; }

        public DateTime TransferDate { get; set; } = DateTime.UtcNow;

        public DateTime? DeliveryDate { get; set; }

        [MaxLength(100)]
        public string? DriverName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<VanInventoryItem> Items { get; set; } = new List<VanInventoryItem>();
    }
}
