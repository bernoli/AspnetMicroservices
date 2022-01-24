using System.Collections.Generic;

namespace Shopping.Aggregator.Models
{
    // Root model response for the Shopping.Aggregator service
    public class ShoppingModel
    {
        public string UserName { get; set; }
        public BasketModel BasketWithProducts { get; set; }
        public IEnumerable<OrderResponseModel> Orders { get; set; }
    }
}