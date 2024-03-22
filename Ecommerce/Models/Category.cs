namespace Ecommerce.Models;
public class Category
{

    public Category(string name, string description = "None")
    {
        Name = name;
        Description = description;
    }
    // basically a tag
    public int Id { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; }
}