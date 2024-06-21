using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models;
public class Rating{
    [Range(1,5,ErrorMessage ="Rating must be between 1 and 5")]
    public int RatingN{get;set;}
    public string? Review{get;set;}

  
    public Guid UserId{get;set;}
    public User user{get;set;}=null!;

  
    public int ProductId{get;set;}    
    public Product Product{get;}=null!;

    public DateTime CreatedAt{get;set;}=DateTime.Now;
    public DateTime UpdatedAt{get;set;}=DateTime.Now;
}