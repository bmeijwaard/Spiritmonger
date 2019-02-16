using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Messages;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Domain.Entities;
using Spiritmonger.Persistence.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Spiritmonger.Core.Services
{
    public abstract class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : IBaseDto
    {
        private readonly ISqlContextProvider _contextProvider;

        public BaseService(ISqlContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<ServiceResponse<IEnumerable<TDto>>> ReadAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            IQueryable<TEntity> query = _contextProvider.Context.Set<TEntity>();

            if (expression != null)
            {
                query = query.Where(expression);
            }

            var entities = await query.ToListAsync().ConfigureAwait(false);
            return new ServiceResponse<IEnumerable<TDto>>(entities.Select(e => Mapper.Map<TDto>(e)));
        }

        public async Task<ServiceResponse<TDto>> ReadAsync(Guid id)
        {
            var entity = await _contextProvider.Context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
            return new ServiceResponse<TDto>(Mapper.Map<TDto>(entity));
        }

        public async Task<ServiceResponse> CreateAsync(TDto dto)
        {
            if (await _contextProvider.Context.Set<TEntity>().AnyAsync(e => e.Id == dto.Id).ConfigureAwait(false))
                return new ServiceResponse($"The given {typeof(TEntity).Name} already exists.");

            return (ServiceResponse)await _contextProvider.ExecuteTransactionAsync(async context =>
            {
                var entity = Mapper.Map<TEntity>(dto);

                await context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);

                return new ServiceResponse();
            }).ConfigureAwait(false);
        }

        public async Task<ServiceResponse> UpdateAsync(TDto dto)
        {
            var entity = await _contextProvider.Context.Set<TEntity>().FindAsync(dto.Id).ConfigureAwait(false);
            if (entity == null)
                return new ServiceResponse($"The requested {typeof(TEntity).Name} does not exist.");

            return (ServiceResponse)await _contextProvider.ExecuteTransactionAsync(async context =>
            {
                context.Attach(entity);

                Mapper.Map(dto, entity);

                context.Set<TEntity>().Update(entity);
                await context.SaveChangesAsync().ConfigureAwait(false);

                return new ServiceResponse();
            }).ConfigureAwait(false);
        }


        public async Task<ServiceResponse> DeleteAsync(TDto dto)
        {
            var entity = await _contextProvider.Context.Set<TEntity>().FindAsync(dto.Id).ConfigureAwait(false);
            if (entity == null)
                return new ServiceResponse($"The requested {typeof(TEntity).Name} does not exist.");

            return (ServiceResponse)await _contextProvider.ExecuteTransactionAsync(async context =>
            {
                context.Attach(entity);

                context.Set<TEntity>().Remove(entity);
                await context.SaveChangesAsync().ConfigureAwait(false);

                return new ServiceResponse();
            }).ConfigureAwait(false);
        }

        public async Task<ServiceResponse> BulkUpdateOrInsertAsync(IEnumerable<TDto> dtos)
        {
            return (ServiceResponse) await _contextProvider.ExecuteTransactionAsync(async context =>
            {
                await context.BulkInsertOrUpdateAsync(dtos.Select(dto => Mapper.Map<TEntity>(dto)).ToList()).ConfigureAwait(false);
                return new ServiceResponse();
            });
        }
    }
}
