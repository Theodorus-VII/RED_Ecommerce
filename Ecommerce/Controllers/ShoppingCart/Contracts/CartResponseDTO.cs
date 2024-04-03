using Ecommerce.Models;
using Ecommerce.Models.ShoppingCart;

namespace Ecommerce.Controllers.ShoppingCart.Contracts
{
    public class CartResponseDTO
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItem>? Items { get; set; }
    }

}
