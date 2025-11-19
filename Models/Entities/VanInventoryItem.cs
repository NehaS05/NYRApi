using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class VanInventoryItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VanInventoryId { get; set; }

        [ForeignKey("VanInventoryId")]
        public virtual VanInventory VanInventory { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [Required]
        public int ProductVariationId { get; set; }

        [ForeignKey("ProductVariationId")]
        public virtual ProductVariation ProductVariation { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
