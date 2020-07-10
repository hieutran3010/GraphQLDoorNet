namespace GraphQLDoorNet
{
    using System;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Extensions;
    using Models;
    using Abstracts;

    public class EntityQueryBase<T> where T : class, IEntityBase, new()
    {
        protected readonly IUnitOfWork UnitOfWork;

        public EntityQueryBase(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public virtual Task<T> GetById(Guid id)
        {
            var repository = this.UnitOfWork.GetRepository<T>();
            return  repository.GetQueryable().FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public virtual Task<T> QueryOne(QueryParams queryParams)
        {
            var repository = this.UnitOfWork.GetRepository<T>();
            return repository.GetQueryable().ApplyQueryOneParams(queryParams);
        }

        public virtual Task<List<T>> QueryMany(QueryParams queryParams)
        {
            var repository = this.UnitOfWork.GetRepository<T>();
            return repository.GetQueryable().ApplyQueryParams(queryParams).ToListAsync();
        }

        public virtual async Task<CountResult> Count(string query)
        {
            var result = new CountResult
            {
                NumberOfItem = string.IsNullOrWhiteSpace(query)
                    ? await this.UnitOfWork.GetRepository<T>().GetQueryable().CountAsync()
                    : await Task.Run(() => this.UnitOfWork.GetRepository<T>().GetQueryable().Count(query))
            };
            return result;
        }
    }
}