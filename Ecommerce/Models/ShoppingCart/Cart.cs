namespace Ecommerce.Models.ShoppingCart
{
    public class Cart
    {
        public int CartId { get; set; }
        public string? UserId { get; set; }
        public float TotalPrice { get; set; }

        // Navigation property for related CartItems
        public List<CartItem>? Items { get; set; }

        public void UpdateTotalPrice()
        {
            if (Items != null)
            {
                TotalPrice = Items.Sum(item => item.Price);
            }
        }
    }
}
