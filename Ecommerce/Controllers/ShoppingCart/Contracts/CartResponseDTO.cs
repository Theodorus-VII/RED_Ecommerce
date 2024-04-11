using Ecommerce.Models;

namespace Ecommerce.Controllers.ShoppingCart.Contracts
{
    public class CartResponseDTO
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemResponseDTO>? Items { get; set; }
    }

    public class CartItemResponseDTO
    {
        public int CartItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
