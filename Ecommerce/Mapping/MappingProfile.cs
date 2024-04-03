using AutoMapper;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Models.ShoppingCart;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Cart, CartResponseDTO>();
        CreateMap<Address, AddressResponseDTO>();
    }
}
