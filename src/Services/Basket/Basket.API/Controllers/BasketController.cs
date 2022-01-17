using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Basket.API.GrpcServices;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<BasketController> _logger;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger, DiscountGrpcService discountGrpcService)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _discountGrpcService = discountGrpcService;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basket = await _basketRepository.GetBasket(username);
            return Ok(basket ?? new ShoppingCart(username));
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {
            foreach (var shoppingCartItem in shoppingCart.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(shoppingCartItem.ProductName);
                shoppingCartItem.Price -= coupon.Amount;
            }
            
            var basket = await _basketRepository.UpdateBasket(shoppingCart);
            return Ok(basket);
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(ActionResult), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string username)
        {
            await _basketRepository.DeleteBasket(username);
            return Ok();
        }
    }
}
