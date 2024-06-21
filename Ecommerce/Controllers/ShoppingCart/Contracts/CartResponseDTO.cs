using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;

namespace Ecommerce.Controllers.ShoppingCart.Contracts
{
    public class CartResponseDTO
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemResponseDTO>? Items { get; set; }
    }

    public class CartItemResponseDTO
    {
        public int CartItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public ProductDto? Product { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public int Count { get; set; } = 1;
        public string Details { get; set; } = "None";
        public Category Category { get; set; } = Category.Other;
        public List<ImageResponseDTO> Images { get; set; } = new List<ImageResponseDTO>();
        public float Price { get; set; }
        public List<Rating> Ratings { get; set; } = new List<Rating>();
    }

    public class ImageResponseDTO
    {
        public string? Url { get; set; }
    }   
}
