using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class DriverAvailabilityDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDriverAvailabilityDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }

    public class UpdateDriverAvailabilityDto
    {
        [Required]
        [MaxLength(20)]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class DriverAvailabilityBulkDto
    {
        [Required]
        public int UserId { get; set; }

        public Dictionary<string, bool> Days { get; set; } = new Dictionary<string, bool>();
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
