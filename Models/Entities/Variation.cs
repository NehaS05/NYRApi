using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.Entities
{
    public class Variation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ValueType { get; set; } = string.Empty; // "Dropdown" or "TextInput"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<VariationOption> Options { get; set; } = new List<VariationOption>();
    }
}
