using Ecommerce.Controllers.Checkout.Contracts;

namespace Ecommerce.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<List<AddressResponseDTO>> GetAddressesAsync(string userId);
        Task<AddressResponseDTO> GetAddressByIdAsync(string userId, int addressId);
        Task<AddressResponseDTO> AddAddressAsync(string userId, AddressRequestDTO address, string addressType);
        Task<AddressResponseDTO> UpdateAddressAsync(string userId, UpdateAddressRequestDTO address);
        Task RemoveAddressAsync(string userId, int addressId);
        Task RemoveMultipleAddresses(string userId, List<int> addressIds);
        Task ClearAddressesAsync(string userId);
    }
}
