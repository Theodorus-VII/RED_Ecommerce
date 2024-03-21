using Ecommerce.Models;

namespace Ecommerce.Models;


public class Order_Item{
    public int Id {get; set;}
    
    public int OrderId {get; set;}
    public int ProductId {get; set;}
    public int Quantity {get; set;}
    public decimal UnitPrice {get; set;} // price of one item

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }

    public void SetQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
