namespace Ecommerce.Controllers.Contracts;
public class FilterAttributesResponse{
    public List<ProductDto> ProductDtos{get;set;}=new List<ProductDto>();
    public int NextIndex{get;set;}
    public int Total{get;set;}
    public float MaximumPrice{get;set;}
}