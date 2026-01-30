using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class LocationInventoryData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        [NotMapped]
        public virtual Location Location { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [NotMapped]
        public virtual Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// The specific product variant (combination of variations)
        /// Nullable for universal products that don't have variants
        /// </summary>
        public int? ProductVariantId { get; set; }

        [ForeignKey("ProductVariantId")]
        [NotMapped]
        public virtual ProductVariant? ProductVariant { get; set; }

        [MaxLength(200)]
        public string? VariationName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }
        [NotMapped]

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        public int? UpdatedBy { get; set; }
        [NotMapped]

        [ForeignKey("UpdatedBy")]
        public virtual User? UpdatedByUser { get; set; }

        public DateTime? UpdatedDate { get; set; }
       
    }
}
