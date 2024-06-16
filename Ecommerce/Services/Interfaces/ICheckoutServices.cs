using Ecommerce.Controllers.Checkout.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<List<AddressResponseDTO>> GetAddressesAsync(string userId, AddressType addressType);
        Task<AddressResponseDTO> GetAddressByIdAsync(string userId, int addressId, AddressType addressType);
        Task<AddressResponseDTO> AddAddressAsync(string userId, AddressRequestDTO address, AddressType addressType);
        Task<AddressResponseDTO> UpdateAddressAsync(string userId, UpdateAddressRequestDTO address, AddressType addressType);
        Task RemoveAddressAsync(string userId, int addressId, AddressType addressType);
        Task RemoveMultipleAddresses(string userId, List<int> addressIds, AddressType addressType);
        Task ClearAddressesAsync(string userId, AddressType addressType);
    }
}
