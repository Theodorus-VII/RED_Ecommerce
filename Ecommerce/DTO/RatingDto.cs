public class RatingDto{
    public int Rating{get;set;}
    public string? Review{get;set;}
}
public class ReviewDto:RatingDto{
   public string Name{get;set;}=null!;
}