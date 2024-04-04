using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Services.Interfaces;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
namespace Ecommerce.Services;
public class ProductService:IProductService{
    private ApplicationDbContext _context;
    private IUserAccountService _userService;

    public ProductService(ApplicationDbContext context, IUserAccountService userService){
        this._context=context;
        this._userService=userService;
    }
    public  async Task<ProductDto?>  GetProduct(int id){
       Product? match=await _context.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==id&&p.Count>0);
       return ToDto(match);
    }
    public ProductDto? ToDto(Product? myProduct){
            if(myProduct==null){
                return null;
            }
            ProductDto myDto=new ProductDto();
            myDto.Id=myProduct.Id;
            myDto.Name=myProduct.Name??string.Empty;
            myDto.Brand=myProduct.Brand??string.Empty;
            myDto.Details=myProduct.Details??"None";
            myDto.Category=myProduct.Category;
            myDto.Price=myProduct.Price;
            myDto.Count=myProduct.Count;
            myDto.Images=myProduct.Images.Select(img=>img.Url).ToList();
            return myDto;
    }
     public async Task<ProductDto> RegisterProduct(ProductDto dto){
        if(dto.Name==null||dto.Brand==null)throw new Exception("Invalid request");
        Product product=new Product{
            Name=dto.Name,
            Brand=dto.Brand,
            Details=dto.Details,
            Category=dto.Category,
            Price=dto.Price,
            Count=dto.Count
        };
        _context.Products?.Add(product);
        await _context.SaveChangesAsync();
        List<Image> images=dto.Images.Select(imgUrl=>new Image{Url=imgUrl,ProudctId=product.Id}).ToList();
        product.Images=images;
        await _context.SaveChangesAsync();
        dto.Id=product.Id;
        return dto;
    }
    public async Task<FilterAttributesResponse> GetProductByFilter(FilterAttributes filterAttributes,int start,int maxSize){
        try{
            List<Product> products=await _context.Products.Where(p=>p.Price<=filterAttributes.high&&p.Price>=filterAttributes.low)
                                                      .Where(p=>p.Name.Contains(filterAttributes.name)||p.Brand.Contains(filterAttributes.name))
                                                      .Where(p=>p.Count>0)
                                                      .ToListAsync();
            List<Product> finalProducts=new List<Product>();
            FilterAttributesResponse response=new FilterAttributesResponse();
            
            if(filterAttributes.categories!=null){
                List<string> filterCategories=filterAttributes.categories.ToList();
                foreach(Product product in products){
                    string category=product.Category.ToString();
                    foreach(string filCategory in filterCategories){
                        if(category.Contains(filCategory)){
                            finalProducts.Add(product);
                            break;
                        }
                    }
                   
                }
            }
            else finalProducts=products;
            List<ProductDto> pDto=new List<ProductDto>();
            if(start>=maxSize+start)throw new Exception("Invalid start index");
            if(start+maxSize>=finalProducts.Count){
                maxSize=finalProducts.Count-start;
                response.NextIndex=-1;
            }
            else response.NextIndex=start+maxSize;
            if(start==0)response.Total=finalProducts.Count;
            Console.WriteLine(start);
            Console.WriteLine(maxSize);
            Console.WriteLine("******************");
            finalProducts=finalProducts.GetRange(start,maxSize);
            foreach(Product product in finalProducts ){
                pDto.Add(ToDto(product));
            }
            response.ProductDtos=pDto;
            return response;
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
        
        List<Rating> ratings=await _context.Ratings.Where(r=>r.ProductId==id).ToListAsync();
        if(ratings==null)throw new Exception("Product doesn't exist");
        if(ratings.Count==0)return -1;
        double average=0;
        foreach(Rating rating in ratings)average+=rating.RatingN;
        average/=ratings.Count;
        return average;

    }
    public async Task<ProductDto> ModifyProudct(ProductDto product, int id){
        Product? match=await _context.Products.FindAsync(id);
        if(match==null)throw new Exception();
        match.Name=product.Name??match.Name;
        match.Brand=product.Brand??match.Brand;
        match.Details=product.Details;
        match.Price=product.Price<0?match.Price:product.Price;
        match.Count=product.Count<0?match.Count:product.Count;
        match.Images=product.Images.Select(imgUrl=>new Image{Url=imgUrl,ProudctId=id}).ToList();
        await _context.SaveChangesAsync();
        return ToDto(match);
    }
    public async Task BuyProduct(int id){
        Product? product=await _context.Products.FindAsync(id);
        if(product!=null)product.Count-=1;
        await _context.SaveChangesAsync();
    }
    public async Task AddRating(int id,RatingDto dto, Guid uId){
        Product product=await _context.Products.FirstAsync(p=>p.Id==id);
        if(product==null)throw new Exception("Product doesn't exist");
        Rating? rating=await _context.Ratings.FindAsync(id,uId);
        if(rating!=null){
            rating.RatingN=dto.Rating;
            rating.Review=dto.Review;
        }
        else{
            product?.Ratings.Add(new Rating{
            RatingN=dto.Rating,
            Review=dto.Review,
            ProductId=id,
            UserId=uId
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
            List<Rating>? ratings=await _context.Ratings.Where(r=>r.ProductId==id).ToListAsync();
            List<ReviewDto> reviews=new List<ReviewDto>();
            if(ratings!=null){
                foreach(Rating _rating in ratings){
                    User? user;
                    if(_rating.RatingN>=low&&_rating.RatingN<=high){
                        user=await _userService.GetUserById(_rating.UserId);
                        string fullName=user?.FirstName+" "+user?.LastName;
                        reviews.Add(new ReviewDto{
                        Rating=_rating.RatingN,
                        Review=_rating.Review,
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
        product.Count=count;
        await _context.SaveChangesAsync();
    }
    public async Task<List<string>?> RefreshImages(int id){
        Product? product=await _context.Products.Include(p=>p.Images).FirstAsync(p=>p.Id==id);
        if(product==null)return null;
        List<string> presentImages=await Task.Run(()=>{
            return Directory.EnumerateFiles("./Public/Images",$"*PID{id}*",SearchOption.TopDirectoryOnly).ToList<string>();
        });
        product.Images=presentImages.Select(imgUrl=>new Image{Url=imgUrl,ProudctId=id}).ToList();
        await _context.SaveChangesAsync();
        return presentImages;
    }
    public async Task<List<string>?> PutImages(int id, List<string> imgNames){
        Product? product=await _context.Products.Include(p=>p.Images).FirstAsync(p=>p.Id==id);
        if(product==null)return null;
        try{
            List<string> presentImages=await Task.Run(()=>{
            return Directory.EnumerateFiles("./Public/Images",$"*PID{id}*",SearchOption.TopDirectoryOnly).ToList<string>();
            });
            List<string> toDelete=new List<string>();
            foreach(string prImg in presentImages){
                if(imgNames.Contains(prImg))continue;
                toDelete.Add(prImg);
            }
            await Task.Run(()=>{
                string path="./Public/Images";
                foreach(string toBeDeleted in toDelete){
                    File.Delete(Path.Join(path,toBeDeleted));
                }
            });
        }
        catch{
            throw new Exception("DeletingFileException");
        }
        try{
            product.Images=imgNames.Select(imgN=>new Image{ProudctId=id,Url=imgN}).ToList();
            await _context.SaveChangesAsync();
        }
        catch{
            throw new Exception("SavingFileException");
        }
        
        return imgNames;

    }
    public async Task<byte[]?> GetImage(string name){
        byte[] b;
        string path="./Public/Images";
       
        b=await File.ReadAllBytesAsync(Path.Join(path,name));
        return b;
    }
    



}