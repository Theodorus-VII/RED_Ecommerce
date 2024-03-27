using AutoMapper;
using Ecommerce.Controllers.Cart.Contracts;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CartItem, CartItemResponseDTO>();
    }
}
