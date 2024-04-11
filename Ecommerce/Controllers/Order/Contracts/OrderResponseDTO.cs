using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Controllers.Orders.Contracts
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public string? OrderNumber { get; set; }
        public string? Status { get; set; }
        public AddressResponseDTO? ShippingAddress { get; set; }
        public AddressResponseDTO? BillingAddress { get; set; }
        public PaymentInfo? PaymentInfo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderItemResponseDTO>? Items { get; set; }
    }

    public class OrderItemResponseDTO
    {
        public int OrderItemId { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Product? Product { get; set; }
    }
}
