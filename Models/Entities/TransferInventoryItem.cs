using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class TransferInventoryItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TransferInventoryId { get; set; }

        [ForeignKey("TransferInventoryId")]
        public virtual TransferInventory TransferInventory { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        public int? ProductVariationId { get; set; }

        [ForeignKey("ProductVariationId")]
        public virtual ProductVariation? ProductVariation { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
