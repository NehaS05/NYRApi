using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class VanDto
    {
        public int Id { get; set; }
        public string DefaultDriverName { get; set; } = string.Empty;
        public string VanName { get; set; } = string.Empty;
        public string VanNumber { get; set; } = string.Empty;
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateVanDto
    {
        [Required]
        [MaxLength(100)]
        public string DefaultDriverName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string VanName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VanNumber { get; set; } = string.Empty;
        
        public int? DriverId { get; set; }
    }

    public class UpdateVanDto
    {
        [Required]
        [MaxLength(100)]
        public string DefaultDriverName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string VanName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VanNumber { get; set; } = string.Empty;
        
        public int? DriverId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}


