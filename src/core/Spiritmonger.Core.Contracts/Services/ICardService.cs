using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spiritmonger.Core.Contracts.Services
{
    public interface ICardService : IBaseService<Card, CardDTO>
    {
        Task<(IList<CardDTO> response, int totalCount)> SearchAsync(string searchParam, int? skip = null, int? take = null, bool relevance = false);
    }
}
