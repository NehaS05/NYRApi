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

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU2 { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU3 { get; set; }

        [MaxLength(100)]
        public string? BarcodeSKU4 { get; set; }

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

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool ShowInCatalogue { get; set; } = true;

        public bool IsUniversal { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<ProductVariation> Variations { get; set; } = new List<ProductVariation>();
    }
}
