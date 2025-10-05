using System;

namespace NYR.API.Models.Entities
{
    public class Van
    {
        public int Id { get; set; }
        public string DefaultDriverName { get; set; } = string.Empty;
        public string VanName { get; set; } = string.Empty;
        public string VanNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


