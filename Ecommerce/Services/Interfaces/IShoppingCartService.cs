using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Controllers.Cart.Contracts;
using Ecommerce.Models; // Replace with your actual model namespace

public interface IShoppingCartService
{
    Task AddToCartAsync(string userId, int productId, int quantity);
    Task AddMultipleItemsToCartAsync(string userId, List<AddToCartRequest> items);
    Task RemoveFromCartAsync(string userId, int productId);
    Task UpdateCartItemQuantityAsync(string userId, int productId, int newQuantity);
    Task<List<CartItemResponseDTO>> GetCartItemsAsync(string userId);
    Task DeleteCartItemsAsync(string userId, List<int> cartItemIds);
    Task ClearCartAsync(string userId);
}
