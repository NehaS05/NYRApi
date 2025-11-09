using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class ProductVariation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string VariationType { get; set; } = string.Empty; // Size, Color, Material, etc.

        [Required]
        [MaxLength(100)]
        public string VariationValue { get; set; } = string.Empty; // Small, Red, Cotton, etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
