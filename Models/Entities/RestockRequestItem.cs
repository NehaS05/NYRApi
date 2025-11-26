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

        public int? ProductVariationId { get; set; }

        [ForeignKey("ProductVariationId")]
        public virtual ProductVariation? ProductVariation { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
