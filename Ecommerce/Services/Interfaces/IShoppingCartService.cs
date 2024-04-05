using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Controllers.ShoppingCart.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<CartResponseDTO> GetCartItemsAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task AddMultipleItemsToCartAsync(string userId, List<AddToCartRequest> items);
        Task RemoveFromCartAsync(string userId, int productId);
        Task UpdateCartItemQuantityAsync(string userId, int productId, int newQuantity);
        Task RemoveMultipleItemsFromCartAsync(string userId, List<int> cartItemIds);
        Task ClearCartAsync(string userId);
    }
}