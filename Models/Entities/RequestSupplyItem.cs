using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class RequestSupplyItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequestSupplyId { get; set; }

        [ForeignKey("RequestSupplyId")]
        public virtual RequestSupply RequestSupply { get; set; } = null!;

        [Required]
        public int VariationId { get; set; }

        [ForeignKey("VariationId")]
        public virtual VariationOption Variation { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
