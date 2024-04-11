using Ecommerce.Controllers.Orders.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<string> MakeOrderAsync(string userId, int paymentInfoId, int shippingAddressId, int billingAddressId);
        Task<List<OrderResponseDTO>> GetOrdersAsync(string userId);
        Task<OrderItemResponseDTO> GetOrderAsync(string userId, int orderId);
        Task<OrderItemResponseDTO> GetOrderByOrderNumberAsync(string orderNumber);
        Task UpdateOrderStatusAsync(int orderId, string status);

    }
}
