using AutoMapper;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Controllers.Orders.Contracts;
using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderItem, OrderItemResponseDTO>();
            CreateMap<Order, OrderResponseDTO>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<Cart, CartResponseDTO>();
            CreateMap<CartItem, CartItemResponseDTO>();
            CreateMap<ShippingAddress, AddressResponseDTO>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.ShippingAddressId));
            CreateMap<BillingAddress, AddressResponseDTO>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.BillingAddressId));
        }
    }
}