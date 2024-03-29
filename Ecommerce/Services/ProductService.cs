using Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Services.Interfaces;
namespace Ecommerce.Models;
public interface IProductService{
   public Task<ProductDto?> GetProduct(int id);
    public Task<ProductDto> RegisterProduct(ProductDto dto);
    public Task DeleteProduct(int id);
    // public Task<float> GetAverageRating(int id);
    public Task<List<ProductDto>?> GetProductByFilter(FilterAttributes filterAttributes, int start, int maxSize);
    public Task<ProductDto> ModifyProudct(ProductDto product,int id);  
    public Task BuyProduct(int id);
    public Task<double> GetAverageRating(int id);
    public Task AddRating(int id,RatingDto dto, Guid uId);
    public Task DeleteRating(int id, Guid uId);
    public Task<List<ReviewDto>> GetProductReviews(int id,int low,int high);

}
public class ProductService:IProductService{
    private ApplicationDbContext _context;
    private IUserAccountService _userService;

    public ProductService(ApplicationDbContext context, IUserAccountService userService){
        this._context=context;
        this._userService=userService;
    }
    public  async Task<ProductDto?>  GetProduct(int id){
       Product? match=await _context.Products.FirstOrDefaultAsync(p=>p.Id==id&&p.count>0);
       return ToDto(match);
    }
    public ProductDto? ToDto(Product? myProduct){
            if(myProduct==null){
                return null;
            }
            ProductDto myDto=new ProductDto();
            myDto.Id=myProduct.Id;
            myDto.Name=myProduct.name??string.Empty;
            myDto.Brand=myProduct.brand??string.Empty;
            myDto.Details=myProduct.details??"None";
            myDto.Category=myProduct.category;
            myDto.Price=myProduct.price;
            myDto.Image=myProduct.image??myDto.Image;
            return myDto;
    }
     public async Task<ProductDto> RegisterProduct(ProductDto dto){
        if(dto.Name==null||dto.Brand==null)throw new Exception("Invalid request");
        Product product=new Product{
            name=dto.Name,
            brand=dto.Brand,
            details=dto.Details,
            category=dto.Category,
            image=dto.Image,
            price=dto.Price,
            count=dto.Count
        };
        _context.Products?.Add(product);
        await _context.SaveChangesAsync();
        dto.Id=product.Id;
        return dto;
    }
    public async Task<List<ProductDto>?> GetProductByFilter(FilterAttributes filterAttributes,int start,int maxSize){
        try{
            List<Product> products=await _context.Products.Where(p=>p.price<=filterAttributes.high&&p.price>=filterAttributes.low)
                                                      .Where(p=>p.name.Contains(filterAttributes.name)||p.brand.Contains(filterAttributes.name))
                                                      .Where(p=>p.count>0)
                                                      .ToListAsync();
            List<Product> finalProducts=new List<Product>();
            string? filterCategory=filterAttributes.category.ToString();
            if(filterCategory!=null){
                foreach(Product product in products){
                    string category=product.category.ToString();
                    if(category.Contains(filterCategory)){
                        finalProducts.Add(product);
                    }
                }

            }
            List<ProductDto> pDto=new List<ProductDto>();
            if(start>=maxSize)throw new Exception("Invalid start index");
            if(start+maxSize>products.Count)maxSize=finalProducts.Count-start;
            finalProducts=finalProducts.GetRange(start,start+maxSize);
            foreach(Product product in finalProducts ){
                pDto.Add(ToDto(product));
            }
            return pDto;
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return null;
        }
    }
     public async Task DeleteProduct(int id){
        Product? product=await _context.Products.SingleAsync(p=>p.Id==id);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
    public async Task<double> GetAverageRating(int id){
        
        List<Rating> ratings=await _context.Ratings.Where(r=>r.productId==id).ToListAsync();
        if(ratings==null)throw new Exception("Product doesn't exist");
        if(ratings.Count==0)return -1;
        double average=0;
        foreach(Rating rating in ratings)average+=rating.rating;
        average/=ratings.Count;
        return average;

    }
    public async Task<ProductDto> ModifyProudct(ProductDto product, int id){
        Product? match=await _context.Products.FindAsync(id);
        if(match==null)throw new Exception();
        match.name=product.Name??match.name;
        match.brand=product.Brand??match.brand;
        match.details=product.Details;
        match.image=product.Image;
        match.price=product.Price;
        match.count=product.Count;
        await _context.SaveChangesAsync();
        return ToDto(match);
    }
    public async Task BuyProduct(int id){
        Product? product=await _context.Products.FindAsync(id);
        if(product!=null)product.count-=1;
        await _context.SaveChangesAsync();
    }
    public async Task AddRating(int id,RatingDto dto, Guid uId){
        Product product=await _context.Products.FirstAsync(p=>p.Id==id);
        if(product==null)throw new Exception("Product doesn't exist");
        Rating? rating=await _context.Ratings.FindAsync(id,uId);
        if(rating!=null){
            rating.rating=dto.Rating;
            rating.review=dto.Review;
        }
        else{
            product?.ratings.Add(new Rating{
            rating=dto.Rating,
            review=dto.Review,
            productId=id,
            userId=uId
        });

        }
        
        // _context.Ratings.Add(new Rating{
        //     rating=dto.Rating,
        //     review=dto.Review,
        //     ProductId=id,
        //     Email=email
        // });
        await _context.SaveChangesAsync();
    }
    public async Task DeleteRating(int id, Guid uId){
        Rating? rating=await _context.Ratings.FindAsync(id,uId);
        if(rating!=null){
            _context.Ratings.Remove(rating);
        }
        else throw new Exception("Your review wasn't found");
        await _context.SaveChangesAsync();
    }
    public async Task<List<ReviewDto>> GetProductReviews(int id,int low,int high){
        try{
            List<Rating>? ratings=await _context.Ratings.Where(r=>r.productId==id).ToListAsync();
            List<ReviewDto> reviews=new List<ReviewDto>();
            if(ratings!=null){
                foreach(Rating _rating in ratings){
                    User? user;
                    if(_rating.rating>=low&&_rating.rating<=high){
                        user=await _userService.GetUserById(_rating.userId);
                        string fullName=user?.FirstName+" "+user?.LastName;
                        reviews.Add(new ReviewDto{
                        Rating=_rating.rating,
                        Review=_rating.review,
                        Name=fullName
                        });
                    }  
                }
            }
            return reviews;
        }
        catch{
            throw new Exception("Something went wrong whie trying to get reviews");
        }
        
        
    }
    public async Task SetCount(int id,int count){
        Product? product=await _context.Products.FindAsync(id);
        if(product==null)throw new Exception("Product doesn't exist");
        if(count<0)throw new InvalidDataException();
        product.count=count;
        await _context.SaveChangesAsync();
    }


}