namespace GraphQLDoorNet
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Linq.Dynamic.Core;
    using System.Linq;
    using Models;
    using Abstracts;

    public class EntityMutationBase<T, TInput> where T : class, IEntityBase, new()
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IInputMapper InputMapper;

        public EntityMutationBase(IUnitOfWork unitOfWork, IInputMapper inputMapper)
        {
            this.UnitOfWork = unitOfWork;
            this.InputMapper = inputMapper;
        }

        public virtual async Task<T> Add(TInput input)
        {
            var repository = this.UnitOfWork.GetRepository<T>();
            var entity = this.InputMapper.Map<TInput, T>(input);
            await repository.AddAsync(entity);
            await this.UnitOfWork.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> Update(Guid id, TInput input)
        {
            var repository = this.UnitOfWork.GetRepository<T>();
            var oldEntity = await repository.FindAsync(id);
            if (oldEntity != null)
            {
                this.InputMapper.MapUpdate<TInput, T>(input, oldEntity);
                await this.UnitOfWork.SaveChangesAsync();
            }

            return oldEntity;
        }

        public virtual async Task<HttpStatus> Delete(Guid id)
        {
            var repository = this.UnitOfWork.GetRepository<T>();
            var entity = await repository.FindAsync(id);
            if (entity != null)
            {
                repository.Remove(entity);
                await this.UnitOfWork.SaveChangesAsync();
                return new HttpStatus
                {
                    Id = (int) HttpStatusCode.OK,
                    Code = "OK"
                };
            }

            return new HttpStatus
            {
                Id = (int) HttpStatusCode.BadRequest,
                Code = "BadRequest"
            };
        }

        public virtual async Task<HttpStatus> AddBatch(TInput[] inputs)
        {
            if (inputs == null)
            {
                throw new HttpRequestException("Invalid Inputs");
            }

            if (inputs.Length > EnvironmentAccessor.Instance.MaximumItemsPerBatch)
            {
                throw new HttpRequestException(
                    $"Only allow {EnvironmentAccessor.Instance.MaximumItemsPerBatch} items per batch");
            }

            var repository = this.UnitOfWork.GetRepository<T>();
            var entities = this.InputMapper.Map<TInput[], T[]>(inputs);
            await repository.AddRangeAsync(entities);
            await this.UnitOfWork.SaveChangesAsync();
            return new HttpStatus
            {
                Id = (int) HttpStatusCode.OK,
                Code = "OK"
            };
        }

        public virtual async Task<HttpStatus> DeleteBatch(Guid[] ids)
        {
            if (ids == null)
            {
                throw new HttpRequestException("Invalid Inputs");
            }

            if (ids.Length > EnvironmentAccessor.Instance.MaximumItemsPerBatch)
            {
                throw new HttpRequestException(
                    $"Only allow {EnvironmentAccessor.Instance.MaximumItemsPerBatch} items per batch");
            }

            var repository = this.UnitOfWork.GetRepository<T>();
            var entities = repository.GetQueryable().Where(entity => ids.Contains(entity.Id));
            if (entities.Any())
            {
                repository.RemoveRange(entities);
                await this.UnitOfWork.SaveChangesAsync();
            }

            return new HttpStatus
            {
                Id = (int) HttpStatusCode.OK,
                Code = "OK"
            };
        }

        public virtual async Task<HttpStatus> DeleteQueryable(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new HttpRequestException("Invalid query");
            }

            var repository = this.UnitOfWork.GetRepository<T>();
            var entities = repository.GetQueryable().Where(query);
            if (entities.Any())
            {
                repository.RemoveRange(entities);
                await this.UnitOfWork.SaveChangesAsync();
            }

            return new HttpStatus
            {
                Id = (int) HttpStatusCode.OK,
                Code = "OK"
            };
        }
    }
}