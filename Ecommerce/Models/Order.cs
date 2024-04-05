namespace Ecommerce.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public DateTime OrderDate { get; set; }
        public float TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
