namespace Ecommerce.Models;

public class Order
{
    public int Id {get; set;}
    public DateTimeOffset OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    // add status enum.
    // add userId as foreign key
    public string UserId { get; set; }
    public User? User { get; set; }
    private readonly List<OrderItem> _items = new List<OrderItem>();

    
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public int GetTotalNumItems => _items.Sum(i => i.Quantity);
    public void AddItemToOrder(int productId, int quantity = 1)
    {
        if (Items.Any(i => i.ProductId == productId))
        {
            Items.First(i => i.ProductId == productId).AddQuantity(quantity);
        }
    }
}