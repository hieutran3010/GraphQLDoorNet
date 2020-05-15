namespace GraphQLDoorNet.QueryBuilder
{
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using Models;

    public class OrderByBuilder<T> : IQueryBuilder<T> where T : class
    {
        public IQueryable<T> Build(QueryParams @params, IQueryable<T> queryable)
        {
            var order = @params.OrderBy;
            if (string.IsNullOrWhiteSpace(order))
            {
                return queryable;
            }

            return queryable.OrderBy($"{order}");
        }
    }
}