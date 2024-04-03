namespace Ecommerce.Models.ShoppingCart
{
    public class Cart
    {
        public int CartId { get; set; }
        public string? UserId { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation property for related CartItems
        public List<CartItem>? Items { get; set; }

        public void UpdateTotalPrice()
        {
            Console.WriteLine("here");
            // Check if Items is not null before performing operations
            if (Items != null)
            {
                // Use SumOrDefault to handle null values in case Product or Price is null
                TotalPrice = Items.Sum(item => item.Price);
                Console.WriteLine("price:here" + TotalPrice.ToString());
            }
            Console.WriteLine("price:"+TotalPrice.ToString());
        }
    }
}
