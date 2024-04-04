using AutoMapper;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Data; 
using Microsoft.EntityFrameworkCore; 

namespace Ecommerce.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<AddressResponseDTO> EnterShippingAddressAsync(string userId, AddressRequestDTO address);
        Task<AddressResponseDTO> EnterBillingAddressAsync(string userId, AddressRequestDTO address);
        Task<List<AddressResponseDTO>> GetAddressesAsync(string userId);
    }

    public class CheckoutService : ICheckoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public CheckoutService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AddressResponseDTO> EnterShippingAddressAsync(string userId, AddressRequestDTO address)
        {
            var newAddress = new Address
            {
                UserId = userId,
                AddressType = AddressType.Shipping,
                Street = address.Street,
                City = address.City,
                State = address.State,
                Country = address.Country,
                PostalCode = address.PostalCode
            };
            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();
            return _mapper.Map<AddressResponseDTO>(newAddress);
           
        }

        public async Task<AddressResponseDTO> EnterBillingAddressAsync(string userId, AddressRequestDTO address)
        {
            var newAddress = new Address
            {
                UserId = userId,
                AddressType = AddressType.Billing,
                Street = address.Street,
                City = address.City,
                State = address.State,
                Country = address.Country,
                PostalCode = address.PostalCode
            };
            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();
            return _mapper.Map<AddressResponseDTO>(newAddress);
        }

        public async Task<List<AddressResponseDTO>> GetAddressesAsync(string userId)
        {
            var Addresses = await _context.Addresses
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<AddressResponseDTO>>(Addresses);
        }
    }



}
