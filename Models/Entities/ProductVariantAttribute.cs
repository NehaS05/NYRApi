using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    /// <summary>
    /// Links a product variant to a specific variation option
    /// Example: ProductVariant "Cotton XL Black" has attributes:
    /// - Material: Cotton (VariationId=1, VariationOptionId=5)
    /// - Size: XL (VariationId=2, VariationOptionId=12)
    /// - Color: Black (VariationId=3, VariationOptionId=20)
    /// </summary>
    public class ProductVariantAttribute
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductVariantId { get; set; }

        [ForeignKey("ProductVariantId")]
        public virtual ProductVariant ProductVariant { get; set; } = null!;

        /// <summary>
        /// The variation type (e.g., Material, Size, Color)
        /// </summary>
        [Required]
        public int VariationId { get; set; }

        [ForeignKey("VariationId")]
        public virtual Variation Variation { get; set; } = null!;

        /// <summary>
        /// The specific option selected (e.g., Cotton, XL, Black)
        /// </summary>
        [Required]
        public int VariationOptionId { get; set; }

        [ForeignKey("VariationOptionId")]
        public virtual VariationOption VariationOption { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
