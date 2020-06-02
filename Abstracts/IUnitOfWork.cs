namespace GraphQLDoorNet.Abstracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntityBase, new();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}