namespace Ecommerce.Models;
public class Rating{
    public int rating{get;set;}
    public int userId{get;set;}
    public int productId{get;set;}
    public User? user{get;}
    public Product? product{get;}
}