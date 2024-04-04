using AutoMapper;
using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Models;
using Ecommerce.Models.ShoppingCart;

namespace Ecommerce.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cart, CartResponseDTO>();
            CreateMap<Address, AddressResponseDTO>();
        }
    }
}