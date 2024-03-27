using Ecommerce.Data; // Update namespace to Ecommerce.Data
using Ecommerce.Models;
using Ecommerce.Utilities;
using Microsoft.EntityFrameworkCore; // Update namespace to Ecommerce.Models

namespace Ecommerce.Services
{
    public interface ICheckoutService
    {
        Task<ServiceResult<string>> EnterAddressAsync(Address address);
        Task<ServiceResult<string>> MakePaymentAsync(PaymentInfo paymentInfo);
        Task<List<Order>> GetOrderHistoryAsync(string userId);
    }

    public class CheckoutService : ICheckoutService
    {
        private readonly ApplicationDbContext _context;

        public CheckoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<string>> EnterAddressAsync(Address address)
        {
            // Add address validation and saving logic here
            return new ServiceResult<string>(true, "Address entered successfully.");
        }

        public async Task<ServiceResult<string>> MakePaymentAsync(PaymentInfo paymentInfo)
        {
            // Add payment processing logic here
            return new ServiceResult<string>(true, "Payment successful.");
        }

        public async Task<List<Order>> GetOrderHistoryAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return orders;
        }
    }



}
