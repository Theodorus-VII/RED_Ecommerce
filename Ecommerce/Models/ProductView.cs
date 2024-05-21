namespace Ecommerce.Models;

public class ProductView
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime Date {get; set;} = DateTime.Now;
    
}