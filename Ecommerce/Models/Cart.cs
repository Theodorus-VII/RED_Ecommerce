namespace Ecommerce.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string? UserId { get; set; }
        public float TotalPrice { get; set; }

        // Navigation property for related CartItems
        public List<CartItem> Items { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Cart()
        {
            // Initialize Items to an empty list if it should never be null
            Items = new List<CartItem>();
        }

        public void UpdateTotalPrice()
        {
            if (Items != null)
            {
                TotalPrice = Items.Sum(item => item.Price);
            }
        }
    }


    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public Product? Product { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
