using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? DBA { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? BusinessPhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? FaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? DBA { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

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

        [Required]
        [MaxLength(100)]
        public string ContactName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(20)]
        public string? BusinessPhone { get; set; }

        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [MaxLength(20)]
        public string? FaxNumber { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Website { get; set; }
    }

    public class UpdateCustomerDto
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? DBA { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

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

        [Required]
        [MaxLength(100)]
        public string ContactName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(20)]
        public string? BusinessPhone { get; set; }

        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [MaxLength(20)]
        public string? FaxNumber { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Website { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
