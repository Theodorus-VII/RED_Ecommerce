using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;
public class Rating{
    [Range(1,5,ErrorMessage ="Rating must be between 1 and 5")]
    public int Id {get; set;}
    public int rating{get;set;}
    public string Email{get;set;}=null!;
    public int ProductId{get;set;}

    public User user{get;set;}=null!;
    public Product product{get;}=null!;
}