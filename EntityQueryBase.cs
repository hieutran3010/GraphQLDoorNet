namespace GraphQLDoorNet
{
    using System;
    using System.Linq.Dynamic.Core;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Fless.EntityFramework.Abstract;
    using Extensions;
    using Models;

    public class EntityQueryBase<T> where T : class, IEntityBase, new()
    {
        private readonly IUnitOfWork unitOfWork;

        public EntityQueryBase(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public virtual Task<T> GetById(Guid id)
        {
            var repository = this.unitOfWork.GetRepository<T>();
            return  repository.GetQueryable().FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public virtual Task<T> QueryOne(QueryParams queryParams)
        {
            var repository = this.unitOfWork.GetRepository<T>();
            return repository.GetQueryable().ApplyQueryOneParams(queryParams);
        }

        public virtual Task<List<T>> QueryMany(QueryParams queryParams)
        {
            var repository = this.unitOfWork.GetRepository<T>();
            return repository.GetQueryable().ApplyQueryParams(queryParams).ToListAsync();
        }

        public virtual async Task<CountResult> Count(string query)
        {
            var result = new CountResult
            {
                NumberOfItem = string.IsNullOrWhiteSpace(query)
                    ? await this.unitOfWork.GetRepository<T>().GetQueryable().CountAsync()
                    : await Task.Run(() => this.unitOfWork.GetRepository<T>().GetQueryable().Count(query))
            };
            return result;
        }
    }
}