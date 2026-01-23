using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NYR.API.Helpers
{
    public static class PaginationHelper
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            return query.Skip(skip).Take(pageSize);
        }

        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> query,
            string? sortBy,
            string sortOrder,
            Dictionary<string, Expression<Func<T, object>>> sortFields,
            Expression<Func<T, object>> defaultSortField)
        {
            if (string.IsNullOrWhiteSpace(sortBy) || !sortFields.ContainsKey(sortBy.ToLower()))
            {
                return sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(defaultSortField)
                    : query.OrderBy(defaultSortField);
            }

            var sortField = sortFields[sortBy.ToLower()];
            return sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(sortField)
                : query.OrderBy(sortField);
        }
    }
}
