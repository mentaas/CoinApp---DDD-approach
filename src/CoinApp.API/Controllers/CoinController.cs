using CoinApp.API.DTOs;
using CoinApp.API.Extentions;
using CoinApp.API.Interfaces;
using CoinApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CoinController : ControllerBase
    {
        private readonly ICoinService _service;
        public CoinController(ICoinService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("getCoins")]
        [SwaggerOperation(Summary = "Get Coins", Description = "This endpoint gets coins from third-party app.")]
        [ProducesResponseType(typeof(List<Coin>), 200)]
        public async Task<IActionResult> GetCoins() => await _service.GetCoins();

        [HttpGet("getFavCoins")]
        [SwaggerOperation(Summary = "Get Fav Coins", Description = "This endpoit is used to get favorite Conis of authenticated user.")]
        [ProducesResponseType(typeof(List<Coin>), 200)]
        public async Task<IActionResult> GetFavConis() => await _service.GetFavoriteCoins();

        [HttpPut("markOrUnmarkCoin{id}")]
        [SwaggerOperation(Summary = "Mark/Unmark Coin", Description = "This endpoit is used to mark or unmark favorite Coin.")]
        [ProducesResponseType(typeof(List<Coin>), 200)]
        public async Task<IActionResult> MarkCoin(string id) => await _service.SaveRemoveCoinAsFavorite(id);



    }
}
