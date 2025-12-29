using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NYR.API.Models.Entities
{
    public class Van
    {
        public int Id { get; set; }
        public string DefaultDriverName { get; set; } = string.Empty;
        public string VanName { get; set; } = string.Empty;
        public string VanNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Foreign key to User table - represents the assigned driver
        /// </summary>
        public int? DriverId { get; set; }
        
        [ForeignKey("DriverId")]
        public virtual User? Driver { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


