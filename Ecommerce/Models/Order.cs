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
        public int? BillingAddressId { get; set; }
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

        public string GenerateOrderEmailMessage(string customerName)
        {
            StringBuilder message = new();
            message.AppendLine($"<h3>Dear {customerName},</h3>");
            message.AppendLine($"<p>Thank you for shopping with us. Your order details are as follows:</p>");
            message.AppendLine($"<ul><li>Order Number: {OrderNumber}.</li>");
            message.AppendLine($"<li>Order Date: {OrderDate}.</li>");

            message.AppendLine("<li>Ordered Items:<ol>");
            for (int i = 0; i < OrderItems?.Count; i++)
            {
                message.AppendLine($"<li>{OrderItems[i].Product?.Name} - Quantity: {OrderItems[i].Quantity} - Price per unit: {PaymentInfo?.Currency} {OrderItems[i].Product?.Price}.</li>");
            }

            message.AppendLine($"</ol></li><li>Total Price: {PaymentInfo?.Currency} {PaymentInfo?.Amount}.</li></ul>");
            message.AppendLine("<p>If you have any questions or need further assistance, please don't hesitate to contact us.</p>\n");
            message.AppendLine("<p>Thank you for choosing us.</p>\n");
            message.AppendLine("<p>Best regards,</p>");
            message.AppendLine("<p>Red E-commerce</p>");

            return message.ToString();
        }
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public Product? Product { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }

}
