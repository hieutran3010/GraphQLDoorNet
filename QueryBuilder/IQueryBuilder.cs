namespace GraphQLDoorNet.QueryBuilder
{
    using System.Linq;
    using Models;

    public interface IQueryBuilder<T> where T : class
    {
        IQueryable<T> Build(QueryParams @params, IQueryable<T> queryable);
    }
}