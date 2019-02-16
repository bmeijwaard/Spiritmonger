using Microsoft.AspNetCore.Mvc;
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
        private readonly ICardNameService _cardNameService;

        public CardController(ICardService cardService, ICardNameService cardNameService)
        {
            _cardService = cardService;
            _cardNameService = cardNameService;
        }


        [HttpGet("")] 
        public async Task<IActionResult> Get(string namepart)
        {
            var result = await _cardNameService
                .ReadAsync(card => card.Name.Contains(namepart))
                .ConfigureAwait(false);

            //TODO: create generic response object.
            if (!result.Succeeded)
                return StatusCode(500, new { Success = false, result.Error });

            return Ok(new { Success = true, Error = string.Empty, result.Data });
        }
    }
}
