using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Spiritmonger.Api.Models.Requests;
using Spiritmonger.Cmon.Types;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spiritmonger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class MetaCardController : Controller
    {
        private readonly IMetaCardService _cardService;
        private readonly IMemoryCache _memoryCache;

        private const string META_CACHE_KEY = "META_CACHE_KEY";

        public MetaCardController(IMetaCardService cardService, IMemoryCache memoryCache)
        {
            _cardService = cardService;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get all cards.
        /// </summary>
        /// <returns>A collection of (cached) cards</returns>
        [HttpGet("")]
        [Produces(typeof(IEnumerable<MetaCardDTO>))]
        public async Task<IActionResult> Get()
        {
            if (!_memoryCache.TryGetValue(META_CACHE_KEY, out IEnumerable<MetaCardDTO> result))
            {
                var response = await _cardService.ReadAsync();

                result = response.Data;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(7));

                _memoryCache.Set(META_CACHE_KEY, result, cacheEntryOptions);
            }

            return StatusCode(200, new
            {
                Success = true,
                Error = string.Empty,
                Data = result
            });
        }

        /// <summary>
        /// Get all cards by a name part.
        /// </summary>
        /// <param name="name">The name part</param>
        /// <returns>A collection of cards</returns>
        [HttpGet("{name}")]
        [Produces(typeof(IEnumerable<MetaCardDTO>))]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _cardService.ReadAsync(c => c.Name.Contains(name));
            return Ok(new
            {
                Success = result.Succeeded,
                Error = result.Error,
                Data = result.Data,
                TotalCount = result.Data?.Count()
            });
        }

        /// <summary>
        /// Create a new card
        /// </summary>
        /// <remarks>
        /// MetaType:
        /// 
        ///     [Flags]
        ///     public enum MetaType : byte
        ///     {
        ///         None = 0,
        ///         Lands = 1,
        ///         Interaction = 2,
        ///         Manipulation = 4,
        ///         Finisher = 8,
        ///         Ramp = 16
        ///     }
        ///
        /// </remarks>
        /// <param name="input">A MetaCardRequest object</param>
        /// <returns>If succeeded</returns>
        [HttpPost("")]
        [Produces(typeof(object))]
        public async Task<IActionResult> Create(MetaCardRequest input)
        {
            var result = await _cardService.CreateAsync(input.GetDTO());
            if (result.Succeeded) _memoryCache.Remove(META_CACHE_KEY);
            return Ok(new
            {
                Success = result.Succeeded,
                Error = result.Error
            });
        }

        /// <summary>
        /// Update an existing card.
        /// </summary>
        /// <remarks>
        /// MetaType:
        /// 
        ///     [Flags]
        ///     public enum MetaType : byte
        ///     {
        ///         None = 0,
        ///         Lands = 1,
        ///         Interaction = 2,
        ///         Manipulation = 4,
        ///         Finisher = 8,
        ///         Ramp = 16
        ///     }
        ///
        /// </remarks>
        /// <param name="input">A MetaCardRequest object.</param>
        /// <returns>If succeeded</returns>
        [HttpPut("")]
        [Produces(typeof(object))]
        public async Task<IActionResult> Update(MetaCardRequest input)
        {
            var result = await _cardService.UpdateAsync(input.GetDTO());
            if (result.Succeeded) _memoryCache.Remove(META_CACHE_KEY);
            return Ok(new
            {
                Success = result.Succeeded,
                Error = result.Error
            });
        }
    }
}
