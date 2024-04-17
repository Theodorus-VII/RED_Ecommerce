using Ecommerce.Controllers.Contracts;
namespace Ecommerce.Services.Interfaces;
public interface IProductService{
   public Task<ProductDto?> GetProduct(int id);
    public Task<ProductDto> RegisterProduct(ProductDto dto, List<IFormFile> imgFiles);
    public Task DeleteProduct(int id);
    // public Task<float> GetAverageRating(int id);
    public Task<FilterAttributesResponse> GetProductByFilter(FilterAttributes filterAttributes, int start, int maxSize);
    public Task<ProductDto?> ModifyProudct(ProductDto product,int id, List<IFormFile> imgFiles);  
    public Task BuyProduct(int id);
    public Task<double> GetAverageRating(int id);
    public Task AddRating(int id,RatingDto dto, Guid uId);
    public Task DeleteRating(int id, Guid uId);
    public Task<List<ReviewDto>> GetProductReviews(int id,int low,int high,Guid userId);
    public Task<List<string>?> RefreshImages(int id);
   // public Task<byte[]?> GetImage(string name);
   public Task DeleteImages(int id, List<string> imgNames);

}