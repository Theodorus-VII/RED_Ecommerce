using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models;
public class Rating{
    [Range(1,5,ErrorMessage ="Rating must be between 1 and 5")]
    public int rating{get;set;}
    public string? review{get;set;}

  
    public Guid userId{get;set;}
    public User user{get;set;}=null!;

  
    public int productId{get;set;}    
    public Product product{get;}=null!;
}