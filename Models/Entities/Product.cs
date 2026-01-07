using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        [Required]
        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } = null!;

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; } = null!;

        public bool ShowInCatalogue { get; set; } = true;

        public bool IsUniversal { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Product variants - unique combinations of variations
        /// Example: A T-shirt product might have variants like:
        /// - Cotton, XL, Black
        /// - Cotton, XXL, Black
        /// - Polyester, XL, White
        /// </summary>
        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
