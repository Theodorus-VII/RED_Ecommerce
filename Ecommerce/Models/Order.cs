namespace Ecommerce.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public List<OrderItem> OrderItems { get; set; } // Assuming there's an OrderItem model
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        // Other order-related properties as needed
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        // Other order item properties as needed
    }
}
