using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Domain.Entities;
using Spiritmonger.Persistence.Contracts;

namespace Spiritmonger.Core.Services
{
    public class CardService : BaseService<Card, CardDTO>, ICardService
    {
        public CardService(ISqlContextProvider contextProvider) : base(contextProvider)
        {
        }
    }
}
