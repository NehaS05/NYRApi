using NYR.API.Models.DTOs;

namespace NYR.API.Helpers
{
    public static class PaginationServiceHelper
    {
        public static void NormalizePaginationParams(PaginationParamsDto paginationParams)
        {
            // Ensure valid values (DTO defaults handle most cases, but double-check)
            if (paginationParams.PageNumber < 1)
                paginationParams.PageNumber = 1;

            if (paginationParams.PageSize < 1 || paginationParams.PageSize > 100)
                paginationParams.PageSize = 25;

            if (string.IsNullOrWhiteSpace(paginationParams.SortOrder) ||
                (paginationParams.SortOrder.ToLower() != "asc" && paginationParams.SortOrder.ToLower() != "desc"))
                paginationParams.SortOrder = "asc";

            // Trim search term
            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
                paginationParams.Search = paginationParams.Search.Trim();
        }

        public static PagedResultDto<TDto> CreatePagedResult<TDto>(
            IEnumerable<TDto> data,
            int totalCount,
            PaginationParamsDto paginationParams)
        {
            return new PagedResultDto<TDto>
            {
                Data = data.ToList(),
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }
    }
}
