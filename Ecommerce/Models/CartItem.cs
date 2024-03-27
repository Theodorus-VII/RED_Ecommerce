using Ecommerce.Models;

public class CartItem
{
    public int CartItemId { get; set; }
    public string? UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    // Navigation property for the associated product
    public Product? Product { get; set; }
}
