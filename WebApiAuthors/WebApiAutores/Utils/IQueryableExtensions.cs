using WebApiAuthors.DTOs;

namespace WebApiAuthors.Utils
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ToPaginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.PerPage)
                .Take(paginationDTO.PerPage);
        }
    }
}
