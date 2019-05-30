using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Messages;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Domain.Entities;
using Spiritmonger.Persistence.Contracts;
using System;
using System.Threading.Tasks;

namespace Spiritmonger.Core.Services
{
    public class MetaCardService : BaseService<MetaCard, MetaCardDTO>, IMetaCardService
    {
        private readonly ISqlContextProvider _contextProvider;

        public MetaCardService(ISqlContextProvider contextProvider)
            : base(contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public override async Task<ServiceResponse> CreateAsync(MetaCardDTO dto)
        {
            if (await _contextProvider.Context.Set<MetaCard>().AnyAsync(e => e.Name == dto.Name).ConfigureAwait(false))
                return new ServiceResponse($"The given card {dto.Name} already exists.");

            return (ServiceResponse)await _contextProvider.ExecuteTransactionAsync(async context =>
            {
                var entity = Mapper.Map<MetaCard>(dto);

                await context.Set<MetaCard>().AddAsync(entity).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);

                return new ServiceResponse();
            }).ConfigureAwait(false);
        }

        public override async Task<ServiceResponse> UpdateAsync(MetaCardDTO dto)
        {
            if (dto.Id == Guid.Empty)
                return new ServiceResponse($"The requested card does not have a valid id.");

            if (await _contextProvider.Context.Set<MetaCard>().AnyAsync(d => d.Name == dto.Name && d.Id != dto.Id))
                return new ServiceResponse($"The given card {dto.Name} already exists.");

            var entity = await _contextProvider.Context.Set<MetaCard>()
                .FindAsync(dto.Id)
                .ConfigureAwait(false);

            if (entity == null)
                return new ServiceResponse($"The requested card does not exist.");

            return (ServiceResponse)await _contextProvider.ExecuteTransactionAsync(async context =>
            {
                context.Attach(entity);

                Mapper.Map(dto, entity);

                context.Set<MetaCard>().Update(entity);
                await context.SaveChangesAsync().ConfigureAwait(false);

                return new ServiceResponse();
            }).ConfigureAwait(false);
        }

    }
}
