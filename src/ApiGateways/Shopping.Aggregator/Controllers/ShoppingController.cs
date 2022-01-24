using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System.Net;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController: ControllerBase
    {

        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        private readonly ICatalogService _catalogService;

        public ShoppingController(IBasketService basketService, IOrderService orderService, ICatalogService catalogService)
        {
            _basketService = basketService;
            _orderService = orderService;
            _catalogService = catalogService;
        }

        [HttpGet("{userName}", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            var basket = await _basketService.GetBasket(userName);

            foreach (var item in basket.Items)
            {
                var product = await _catalogService.GetCatalog(item.ProductId);
                //set additional product fields on basket item extended model.
                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Summary = product.Summary;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
            }

            var orders = await _orderService.GetOrdersByUserName(userName);

            var shoppingModel = new ShoppingModel()
            {
                UserName = userName,
                BasketWithProducts = basket,
                Orders = orders
            };

            return Ok(shoppingModel);
        }
    }
}
