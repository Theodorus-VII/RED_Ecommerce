namespace Ecommerce.Models;

public class Basket_Item {

    public int Id {get; set;}
    public int BasketId {get; set;}
    public int ProductId {get; set;}
    public int Quantity {get; set;}
    public decimal UnitPrice {get; set;} // price of one item


    public Basket_Item(int productId, int quantity, decimal unitPrice)
    {
        // BasketId = basketId;
        ProductId = productId;
        UnitPrice = unitPrice;
        SetQuantity(quantity);
    }
    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }

    public void SetQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
