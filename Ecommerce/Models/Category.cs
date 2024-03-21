namespace Ecommerce.Models;
public class Category(
    string name,
    string description = "None"
)
{
    // basically a tag
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;
}