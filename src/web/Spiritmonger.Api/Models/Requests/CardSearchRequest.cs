using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spiritmonger.Api.Models.Requests
{
    public class CardSearchRequest : PagedRequest
    {
        public string NamePart { get; set; }
    }
}
