using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Spiritmonger.Api.Models.Requests;
using Spiritmonger.Api.Models.Responses;
using Spiritmonger.Cmon.Constants;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spiritmonger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : Controller
    {
        private readonly ICardService _cardService;
        private readonly IMemoryCache _memoryCache;

        public CardController(ICardService cardService, IMemoryCache memoryCache)
        {
            _cardService = cardService;
            _memoryCache = memoryCache;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(string namepart)
        {
            if (namepart.Length <= 3)
                return StatusCode(400, new { Success = false, Error = "The search requires at least 4 characters." });

            var (response, totalCount) = await _cardService.SearchAsync(namepart, 0, -1, true);
            return Ok(new { Success = true, Error = string.Empty, Data = response, TotalCount = totalCount });
        }

        [HttpPost("")]
        [Produces(typeof(PagedResponse<CardDTO>))]
        public async Task<IActionResult> Search([FromBody] CardSearchRequest request)
        {
            if (request.NamePart.Length <= 3)
                return StatusCode(400, new { Success = false, Error = "The search requires at least 4 characters." });


            var cacheKey = CacheKeys.GetCardSearchCacheKey(request);
            if (!_memoryCache.TryGetValue(cacheKey, out PagedResponse<CardDTO> result))
            {
                var (response, totalCount) = await _cardService.SearchAsync(request.NamePart, request.Skip(), request.Take(), true);

                result = new PagedResponse<CardDTO>(request, totalCount, response);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(7));

                _memoryCache.Set(cacheKey, result, cacheEntryOptions);

                if (!_memoryCache.TryGetValue(CacheKeys.CARDS, out List<string> cached))
                {
                    cached = new List<string>
                    {
                        cacheKey
                    };
                }
                cached.Add(cacheKey);
                _memoryCache.Set(CacheKeys.CARDS, cached, cacheEntryOptions);
            }

            return Ok(new { Success = true, Error = string.Empty, Data = result });
        }
    }
}
