using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    /// <summary>
    /// Represents a unique combination of variations for a product
    /// Example: Product "T-Shirt" -> Variant "Cotton, XL, Black"
    /// </summary>
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        /// <summary>
        /// Display name for this variant (e.g., "Cotton / XL / Black")
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string VariantName { get; set; } = string.Empty;

        /// <summary>
        /// Optional SKU specific to this variant
        /// </summary>
        [MaxLength(100)]
        public string? SKU { get; set; }

        /// <summary>
        /// Optional price override for this variant (if null, uses product base price)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Whether this variant is available for sale
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// The specific variation options that make up this variant
        /// Example: [Material: Cotton, Size: XL, Color: Black]
        /// </summary>
        public virtual ICollection<ProductVariantAttribute> Attributes { get; set; } = new List<ProductVariantAttribute>();
    }
}
