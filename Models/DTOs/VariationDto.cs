using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class VariationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ValueType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<VariationOptionDto> Options { get; set; } = new List<VariationOptionDto>();
    }

    public class VariationOptionDto
    {
        public int Id { get; set; }
        public int VariationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateVariationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ValueType { get; set; } = string.Empty; // "Dropdown" or "TextInput"

        public List<CreateVariationOptionDto> Options { get; set; } = new List<CreateVariationOptionDto>();
    }

    public class CreateVariationOptionDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Value { get; set; }
    }

    public class UpdateVariationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ValueType { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public List<UpdateVariationOptionDto> Options { get; set; } = new List<UpdateVariationOptionDto>();
    }

    public class UpdateVariationOptionDto
    {
        public int? Id { get; set; } // Null for new options

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Value { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
