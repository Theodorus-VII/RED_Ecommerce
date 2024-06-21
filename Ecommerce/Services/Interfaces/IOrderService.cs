using Ecommerce.Controllers.Orders.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<string> MakeOrderAsync(string userId, PaymentInfo paymentInfo, int shippingAddressId, int? billingAddressId);
        Task<List<OrderResponseDTO>> GetOrdersAsync(string userId);
        Task<OrderResponseDTO> GetOrderAsync(string userId, int orderId);
        Task<OrderResponseDTO> GetOrderByOrderNumberAsync(string orderNumber);
        Task UpdateOrderStatusAsync(int orderId, int status);
        Task<List<OrderResponseDTO>> GetRecentOrdersAsync();

    }
}
