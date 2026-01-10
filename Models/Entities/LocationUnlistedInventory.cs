using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class LocationUnlistedInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string BarcodeNo { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int? UpdatedBy { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User? UpdatedByUser { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}