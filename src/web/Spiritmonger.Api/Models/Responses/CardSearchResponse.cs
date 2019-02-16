using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spiritmonger.Api.Models.Requests;
using Spiritmonger.Core.Contracts.DTO;

namespace Spiritmonger.Api.Models.Responses
{
    public class CardSearchResponse : PagedResponse<CardDTO>
    {
        public CardSearchResponse(CardSearchRequest request, int count, IEnumerable<CardDTO> data) 
            : base(request, count, data)
        {
            NamePart = request.NamePart;
        }

        public string NamePart { get; set; }

    }
}
