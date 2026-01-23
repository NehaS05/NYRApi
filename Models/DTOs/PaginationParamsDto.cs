using System.ComponentModel.DataAnnotations;

namespace NYR.API.Models.DTOs
{
    public class PaginationParamsDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize { get; set; } = 25;

        public string? SortBy { get; set; }

        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortOrder must be 'asc' or 'desc'")]
        public string SortOrder { get; set; } = "asc";

        [MaxLength(200)]
        public string? Search { get; set; }
    }
}
