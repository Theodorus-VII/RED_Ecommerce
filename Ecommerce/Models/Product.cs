namespace Ecommerce.Models;

public class Product
{
    public Product(string name = "Unknown",
    string description = "None",
    decimal price = 0.0M,
    string imageUrl = "",
    int inventory = 0){
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        Inventory = inventory;
    }
    public int Id {get; set;}
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int Inventory { get; set; }
    public List<Category> Categories {get; set;} = new List<Category>();
}