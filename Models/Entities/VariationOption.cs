using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class VariationOption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VariationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Value { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("VariationId")]
        public virtual Variation Variation { get; set; } = null!;
    }
}
