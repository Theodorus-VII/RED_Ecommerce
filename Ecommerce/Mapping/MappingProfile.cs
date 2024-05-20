using AutoMapper;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Controllers.Contracts;
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
            CreateMap<Image, ImageResponseDTO>();
            CreateMap<Product, ProductResponseDTO>();
            CreateMap<Cart, CartResponseDTO>();
            CreateMap<CartItem, CartItemResponseDTO>();
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details ?? "None"))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.Url).ToList()));
            CreateMap<ShippingAddress, AddressResponseDTO>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.ShippingAddressId));
            CreateMap<BillingAddress, AddressResponseDTO>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.BillingAddressId));
            CreateMap<PaymentInfo, PaymentInfoDTO>();
        }
    }
}