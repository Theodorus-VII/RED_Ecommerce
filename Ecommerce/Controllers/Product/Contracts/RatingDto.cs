namespace Ecommerce.Controllers.Contracts;
public class RatingDto{
    public bool IsMine{get;set;}=false;
    public int Rating{get;set;}
    public string? Review{get;set;}
}
public class ReviewDto:RatingDto{
   public string Name{get;set;}=null!;
}