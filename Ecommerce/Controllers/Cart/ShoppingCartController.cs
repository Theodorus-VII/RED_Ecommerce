using Ecommerce.Controllers.Cart.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Add this namespace

[Authorize] // Assuming users need to be authenticated to access the cart
[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartController(IShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        // Validate request here if needed

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token
        await _shoppingCartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
        return Ok();
    }

    [HttpPost("add-multiple")]
    public async Task<IActionResult> AddMultipleItemsToCart([FromBody] AddMultipleItemsToCartRequest request)
    {
        // Validate request here if needed

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token
        await _shoppingCartService.AddMultipleItemsToCartAsync(userId, request.Items);
        return Ok();
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
    {
        // Validate request here if needed

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token
        await _shoppingCartService.RemoveFromCartAsync(userId, request.ProductId);
        return Ok();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemQuantityRequest request)
    {
        // Validate request here if needed

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token
        await _shoppingCartService.UpdateCartItemQuantityAsync(userId, request.ProductId, request.NewQuantity);
        return Ok();
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetCartItems()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token

        var cartItems = await _shoppingCartService.GetCartItemsAsync(userId);
        return Ok(cartItems);
    }

    [HttpDelete("remove-multiple")]
    public async Task<IActionResult> DeleteCartItems([FromBody] DeleteCartItemsRequest request)
    {
        // Validate request here if needed

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token
        await _shoppingCartService.DeleteCartItemsAsync(userId, request.CartItemIds);
        return Ok();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get userId from token

        await _shoppingCartService.ClearCartAsync(userId);
        return Ok();
    }
}
