using Spiritmonger.Api.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spiritmonger.Api.Models.Responses
{
    public class PagedResponse<T>
    {
        public PagedResponse(PagedRequest request, int count, IEnumerable<T> data)
        {
            Page = request.Page;
            PageLength = request.PageLength;
            TotalCount = count;
            Data = data;
        }
        public int Page { get; set; }
        public int PageLength { get; set; }

        public int TotalCount { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
