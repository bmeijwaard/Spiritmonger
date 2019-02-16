using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spiritmonger.Api.Models.Requests
{
    public class PagedRequest
    {
        public int Page { get; set; }
        public int PageLength { get; set; }


        public int Skip() => (Page - 1) * PageLength;
        public int Take() => PageLength;
    }
}
