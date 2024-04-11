using System.Text;

namespace Ecommerce.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public string OrderNumber { get; set; } = Guid.NewGuid().ToString();
        public string? Status { get; set; }
        public int ShippingAddressId { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public int BillingAddressId { get; set; }
        public BillingAddress? BillingAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public int PaymentInfoId { get; set; }
        public PaymentInfo? PaymentInfo { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public string GenerateOrderEmailMessage()
        {
            StringBuilder message = new();

            message.AppendLine($"Subject: Order Confirmation - Order Number: {OrderNumber}\n");
            message.AppendLine($"Dear Customer,\n");
            message.AppendLine($"Thank you for shopping with us. Your order details are as follows:\n");
            message.AppendLine($"Order Number: {OrderNumber}");
            message.AppendLine($"Order Date: {OrderDate}\n");

            message.AppendLine("Ordered Items:");
            for (int i = 0; i < OrderItems?.Count; i++)
            {
                message.AppendLine($"{i + 1}. {OrderItems[i].Product?.Name} - Quantity: {OrderItems[i].Quantity} - Price per unit: {OrderItems[i].Product?.Price}");
            }

            message.AppendLine($"\nTotal Price: {PaymentInfo?.Amount}\n");
            message.AppendLine("If you have any questions or need further assistance, please don't hesitate to contact us.\n");
            message.AppendLine("Thank you for choosing us.\n");
            message.AppendLine("Best regards,");
            message.AppendLine("Red E-commerce");

            return message.ToString();
        }
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }

}
