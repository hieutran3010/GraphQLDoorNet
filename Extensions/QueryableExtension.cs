namespace GraphQLDoorNet.Extensions
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using QueryBuilder;

    public static class QueryableExtension
    {
        public static IQueryable<T> ApplyQueryParams<T>(this IQueryable<T> queryable, QueryParams @params)
            where T : class
        {
            if (@params == null)
            {
                return queryable;
            }

            IQueryBuilder<T> expandBuilder = new IncludeBuilder<T>();
            var result = expandBuilder.Build(@params, queryable);

            IQueryBuilder<T> filterBuilder = new QueryBuilder<T>();
            result = filterBuilder.Build(@params, result);

            IQueryBuilder<T> orderBuilder = new OrderByBuilder<T>();
            result = orderBuilder.Build(@params, result);

            IQueryBuilder<T> pagingBuilder = new PagingBuilder<T>();
            result = pagingBuilder.Build(@params, result);

            return result;
        }

        public static Task<T> ApplyQueryOneParams<T>(this IQueryable<T> queryable, QueryParams @params)
            where T : class
        {
            if (@params == null)
            {
                return queryable.FirstOrDefaultAsync();
            }

            IQueryBuilder<T> expandBuilder = new IncludeBuilder<T>();
            var result = expandBuilder.Build(@params, queryable);

            IQueryBuilder<T> filterBuilder = new QueryBuilder<T>();
            result = filterBuilder.Build(@params, result);
            
            IQueryBuilder<T> orderBuilder = new OrderByBuilder<T>();
            result = orderBuilder.Build(@params, result);

            return result.FirstOrDefaultAsync();
        }
    }
}
