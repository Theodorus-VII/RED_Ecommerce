namespace Ecommerce.Models.ShoppingCart
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        // Navigation property for the associated product
        public Product? Product { get; set; }
    }
}