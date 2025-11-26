using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class FollowupRequestDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime FollowupDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateFollowupRequestDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int LocationId { get; set; }
    }

    public class UpdateFollowupRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
