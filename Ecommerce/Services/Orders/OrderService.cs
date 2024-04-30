using AutoMapper;
using Ecommerce.Controllers.Orders.Contracts;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public OrderService(ApplicationDbContext context, IEmailService emailService, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _emailService = emailService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<string> MakeOrderAsync(string userId, int paymentInfoId, int shippingAddressId, int? billingAddressId)
        {
            try
            {
                var cart = await _context.Carts.Where(c => c.UserId == userId)
                    .Include(c => c.Items).ThenInclude(c => c.Product).FirstOrDefaultAsync() ?? throw new ArgumentException("Cart not found");
                var shippingAddress = await _context.ShippingAddresses.Where(a => a.ShippingAddressId == shippingAddressId && a.UserId == userId).FirstOrDefaultAsync() ?? throw new ArgumentException("Shipping address not found");
                var billingAddress = await _context.BillingAddresses.Where(a => a.BillingAddressId == billingAddressId && a.UserId == userId).FirstOrDefaultAsync();
                var newOrder = new Order
                {
                    UserId = userId,
                    OrderItems = cart.Items.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Price,
                        Product = ci.Product
                    }).ToList(),
                    PaymentInfoId = paymentInfoId,
                    ShippingAddress = shippingAddress,
                    BillingAddressId = billingAddressId ?? null,
                    Status = "Pending"
                };
                foreach (var orderItem in newOrder.OrderItems)
                {
                    if (orderItem.Product != null)
                    {
                        int count = orderItem.Product.Count - orderItem.Quantity;
                        if (count < 0) { throw new ArgumentException($"{orderItem.Product.Name} out of stock"); }
                        orderItem.Product.Count = count;
                        _context.Products.Update(orderItem.Product);
                    }

                }
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();
                var user = await _userManager.FindByIdAsync(userId);
                string message = newOrder.GenerateOrderEmailMessage(user.FirstName);
                await _emailService.SendEmail(new EmailDto
                {
                    Subject = $"Order Confirmation - Order Number: {newOrder.OrderNumber}",
                    Recipient = user.Email,
                    Message = message
                });
                return newOrder.OrderNumber;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<OrderResponseDTO>> GetOrdersAsync(string userId)
        {
            try
            {
                var orders = await _context.Orders.Where(o => o.UserId == userId).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Include(o => o.PaymentInfo).Include(o => o.ShippingAddress).Include(o => o.BillingAddress).ToListAsync();
                var orderResponses = _mapper.Map<List<OrderResponseDTO>>(orders);
                for (int i = 0; i < orderResponses.Count; i++)
                {
                    orderResponses[i].StatusInt = orders[i].Status == "Pending" ? 1 : orders[i].Status == "Shipped" ? 2 : 3;
                    var user = await _userManager.FindByIdAsync(orders[i].UserId);
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault("");
                    orderResponses[i].User = new UserDto(
                        user,
                        role
                    );
                    orderResponses[i].User = new UserDto(
                        user,
                        role
                    );
                }
                return orderResponses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderResponseDTO> GetOrderAsync(string userId, int orderId)
        {
            try
            {
                var order = await _context.Orders.Where(o => o.UserId == userId && o.OrderId == orderId).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Include(o => o.PaymentInfo).Include(o => o.ShippingAddress).Include(o => o.BillingAddress).FirstOrDefaultAsync() ?? throw new ArgumentException("Order not found");
                var orderResponse = _mapper.Map<OrderResponseDTO>(order);
                orderResponse.StatusInt = order.Status == "Pending" ? 1 : order.Status == "Shipped" ? 2 : 3;
                return orderResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderResponseDTO> GetOrderByOrderNumberAsync(string orderNumber)
        {
            try
            {
                var order = await _context.Orders.Where(o => o.OrderNumber == orderNumber).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Include(o => o.PaymentInfo).Include(o => o.ShippingAddress).Include(o => o.BillingAddress).FirstOrDefaultAsync() ?? throw new ArgumentException("Order not found");
                var orderResponse = _mapper.Map<OrderResponseDTO>(order);
                orderResponse.StatusInt = order.Status == "Pending" ? 1 : order.Status == "Shipped" ? 2 : 3;
                return orderResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, int status)
        {
            try
            {
                var availableStatuses = new List<string> { "Pending", "Shipped", "Delivered" };
                var order = await _context.Orders.Where(o => o.OrderId == orderId).FirstOrDefaultAsync() ?? throw new ArgumentException("Order not found");
                order.Status = availableStatuses[status - 1];
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<OrderResponseDTO>> GetRecentOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .Where(order => order.Status == "Pending")
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Include(o => o.PaymentInfo)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.BillingAddress)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
                    
                if(orders.Count == 0)
                {
                    return new List<OrderResponseDTO> { };
                }
                // var response = _mapper.Map<List<OrderResponseDTO>>(orders);
                var response = new List<OrderResponseDTO> {};
                foreach (var order in orders)
                {
                    var orderResponse = _mapper.Map<OrderResponseDTO>(order);
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault("");
                    orderResponse.User = new UserDto(
                        user,
                        role
                    );
                    orderResponse.User = new UserDto(
                        user,
                        role
                    );
                    
                    orderResponse.StatusInt = order.Status == "Pending" ? 1 : order.Status == "Shipped" ? 2 : 3;
                    
                    response.Add(orderResponse);
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
