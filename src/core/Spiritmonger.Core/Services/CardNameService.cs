using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Domain.Entities;
using Spiritmonger.Persistence.Contracts;

namespace Spiritmonger.Core.Services
{
    public class CardNameService : BaseService<CardName, CardNameDTO>, ICardNameService
    {
        public CardNameService(ISqlContextProvider contextProvider) : base(contextProvider)
        {
        }
    }
}
