namespace Ecommerce.Models;

public class Product(
    string name = "Unknown",
    string description = "None",
    decimal price = 0.0M,
    string imageUrl = "",
    int inventory = 0
        )
{
    public int Id {get; set;}
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public decimal Price { get; set; } = price;
    public string ImageUrl { get; set; } = imageUrl;
    public int Inventory { get; set; } = inventory;
    public List<Category> Categories {get; set;} = new List<Category>();
}