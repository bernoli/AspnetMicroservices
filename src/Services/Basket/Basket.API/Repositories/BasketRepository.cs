using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    // In this project we are using the DAL layer directly by using the IDistributedCache that actually encapsulate this. 
    // Since we are using the Microsoft StackExchangeRedis service, the implementation is already provided for us. 
    // In contrast with the Catalog.API project where we use mongodb and we implemented the DAL ourselves. 
    public class BasketRepository: IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task<ShoppingCart> GetBasket(string username)
        {
            var basket = await _redisCache.GetStringAsync(username);
            if (string.IsNullOrEmpty(basket))
                return null;

            var shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(basket);
            return shoppingCart;
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
            return await GetBasket(basket.UserName);
        }

        public async Task DeleteBasket(string username)
        {
            await _redisCache.RemoveAsync(username, CancellationToken.None);
        }
    }
}
