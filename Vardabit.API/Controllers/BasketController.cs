using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vardabit.API.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Service.Interfaces;

namespace Vardabit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBasketForUser(int userId)
        {
            var items = await _basketService.GetUserBasketAsync(userId);

            return Ok(items);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToBasket([FromBody] CreateBasketDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var basket = new Basket
            {
                ProductId = dto.ProductId,
                Amount = dto.Amount,
                UserId = dto.UserId
            };

            await _basketService.AddToBasketAsync(basket);

            return StatusCode(201);
        }

        [Authorize]
        [HttpDelete("{basketId}")]
        public async Task<IActionResult> RemoveFromBasket(int basketId)
        {
            await _basketService.RemoveFromBasketAsync(basketId);

            return Ok();
        }
    }
}
