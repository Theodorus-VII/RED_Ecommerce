namespace Ecommerce.Models;

public class Basket
{
    public int Id {get; set;}
    public User? User { get; set; } // User is not in the db, this just set's UserId as a foreign key in EF Core.
    private readonly List<Basket_Item> _items = new List<Basket_Item>();

    public IReadOnlyCollection<Basket_Item> Items => _items.AsReadOnly();
    public int GetTotalNumItems => _items.Sum(i => i.Quantity);

    
    public void AddItemToBasket(int productId, decimal unitPrice, int quantity = 1)
    {
        if (Items.Any(i => i.ProductId == productId))
        {
            Items.First(i => i.ProductId == productId).AddQuantity(quantity);
        }
        else
        {
            _items.Add(new Basket_Item(productId, quantity, unitPrice));
        }
        return;
    }
}