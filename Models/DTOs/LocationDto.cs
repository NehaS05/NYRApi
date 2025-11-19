using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Title { get; set; }
        public string? LocationPhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? FaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateLocationDto
    {
        [Required]
        public int CustomerId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string LocationName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? AddressLine2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(20)]
        public string? LocationPhone { get; set; }

        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [MaxLength(20)]
        public string? FaxNumber { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }
    }

    public class UpdateLocationDto
    {
        [Required]
        public int CustomerId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string LocationName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? AddressLine2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(20)]
        public string? LocationPhone { get; set; }

        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [MaxLength(20)]
        public string? FaxNumber { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
