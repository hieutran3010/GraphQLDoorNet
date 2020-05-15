namespace GraphQLDoorNet.QueryBuilder
{
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using Models;

    public class QueryBuilder<T> : IQueryBuilder<T> where T : class
    {
        public IQueryable<T> Build(QueryParams @params, IQueryable<T> queryable)
        {
            var filter = @params.Query;
            if (string.IsNullOrWhiteSpace(filter))
            {
                return queryable;
            }

            return queryable.Where(filter);
        }
    }
}