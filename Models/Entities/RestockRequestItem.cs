using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class RestockRequestItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RestockRequestId { get; set; }

        [ForeignKey("RestockRequestId")]
        public virtual RestockRequest RestockRequest { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        /// <summary>
        /// The specific product variant (combination of variations)
        /// Nullable for universal products that don't have variants
        /// </summary>
        public int? ProductVariantId { get; set; }

        [ForeignKey("ProductVariantId")]
        public virtual ProductVariant? ProductVariant { get; set; }

        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// The quantity that was actually delivered for this item
        /// Nullable - will be null until delivery is completed
        /// </summary>
        public int? DeliveredQuantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
