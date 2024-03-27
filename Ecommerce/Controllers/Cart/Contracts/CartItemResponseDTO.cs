using Ecommerce.Models;

namespace Ecommerce.Controllers.Cart.Contracts
{
    public class CartItemResponseDTO
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        // Other properties as needed
    }
}
