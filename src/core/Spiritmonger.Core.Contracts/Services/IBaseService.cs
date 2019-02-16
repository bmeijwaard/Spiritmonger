using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Messages;
using Spiritmonger.Domain.Entities;

namespace Spiritmonger.Core.Contracts.Services
{
    public interface IBaseService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : IBaseDto
    {
        Task<ServiceResponse> CreateAsync(TDto dto);
        Task<ServiceResponse> DeleteAsync(TDto dto);
        Task<ServiceResponse<IEnumerable<TDto>>> ReadAsync(Func<TEntity, bool> expression = null);
        Task<ServiceResponse<TDto>> ReadAsync(Guid id);
        Task<ServiceResponse> UpdateAsync(TDto dto);
    }
}