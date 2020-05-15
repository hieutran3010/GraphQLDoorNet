namespace GraphQLDoorNet.QueryBuilder
{
    using System.Linq;
    using Models;

    public class PagingBuilder<T> : IQueryBuilder<T> where T : class
    {
        public IQueryable<T> Build(QueryParams @params, IQueryable<T> queryable)
        {
            var pageNumber = @params.Page;
            var pageSize = @params.PageSize;
            if (!pageSize.HasValue || !pageNumber.HasValue || pageNumber <= 0 || pageSize <= 0)
            {
                return queryable;
            }

            var skipCount = (pageNumber.Value - 1) * pageSize.Value;

            return queryable.Skip(skipCount).Take(pageSize.Value);
        }
    }
}