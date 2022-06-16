using Microsoft.EntityFrameworkCore;

namespace WebApiAuthors.Utils
{
    public static class HtttpContextExtensions
    {
        public async static Task InsertPaginationParametersInHeaders<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double quantity = await queryable.CountAsync();
            httpContext.Response.Headers.Add("X-Total-Count", quantity.ToString());
        }
    }
}
