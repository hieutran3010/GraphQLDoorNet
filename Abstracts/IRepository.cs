namespace GraphQLDoorNet.Abstracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<T>
        where T : class, IEntityBase, new()
    {
        IQueryable<T> GetQueryable(bool asNoTracking = true);

        Task<T> FindAsync(Guid id);

        Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

        Task AddRangeAsync(params T[] entities);

        Task<int> ExecuteSqlRawAsync(string sql);

        IQueryable<T> GetByRawSql(string sql);

        /// <summary>
        /// Begins tracking the given entity in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Modified" /> state such that it will
        /// be updated in the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
        /// </summary>
        void Update(T entity);

        void Remove(T entity);
        
        void RemoveRange(IEnumerable<T> entities);
    }
}