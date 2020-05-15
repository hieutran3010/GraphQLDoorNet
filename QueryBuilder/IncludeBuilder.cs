namespace GraphQLDoorNet.QueryBuilder
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class IncludeBuilder<T> : IQueryBuilder<T> where T : class
    {
        public IQueryable<T> Build(QueryParams @params, IQueryable<T> queryable)
        {
            var expand = @params.Include;
            if (string.IsNullOrWhiteSpace(expand))
            {
                return queryable;
            }

            var result = queryable;
            var expandFields = expand.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim());
            foreach (var expandField in expandFields)
            {
                result = result.Include(expandField);
            }

            return result;
        }
    }
}