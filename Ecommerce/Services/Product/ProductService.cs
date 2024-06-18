using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Services.Interfaces;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using System.Xml;
using Org.BouncyCastle.Utilities;
namespace Ecommerce.Services;
public class ProductService:IProductService{
    private ApplicationDbContext _context;
    private IUserAccountService _userService;

    public ProductService(ApplicationDbContext context, IUserAccountService userService){
        this._context=context;
        this._userService=userService;
    }
    public  async Task<ProductDto?>  GetProduct(int id){
       Product? match=await _context.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==id&&p.Count>-1);
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
     public async Task<ProductDto> RegisterProduct(ProductDto dto, List<IFormFile> imgFiles){
        try{
            if(dto.Name==null||dto.Brand==null) throw new Exception("Invalid request");
            if(dto.Count<0) dto.Count=0;
            if(dto.Price<0) dto.Price=0;
            // List<Image> images=dto.Images.Select(imgUrl=>new Image{Url=imgUrl}).ToList();
           
            List<Image> images=new List<Image>();
            Product product=new Product{
                Name=dto.Name,
                Brand=dto.Brand,
                Details=dto.Details,
                Category=dto.Category,
                Price=dto.Price,
                Count=dto.Count,
            };
            string fileName;
            string extension;
            foreach(IFormFile imgFile in imgFiles){
                fileName=Path.GetRandomFileName();
                extension=Path.GetExtension(imgFile.FileName);
                using(FileStream fs=new FileStream($"Public/Images/{fileName}{extension}",FileMode.Create)){
                    await imgFile.CopyToAsync(fs);
                }
                images.Add(new Image{Url=$"{fileName}{extension}"});
            }
            product.Images.AddRange(images);
            _context.Products?.Add(product);

            await _context.SaveChangesAsync();
            _context.Products=null!;
            // Product? registeredProduct=await _context.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==product.Id);
            // Console.WriteLine("*****************");
            // if(registeredProduct!=null)registeredProduct.Images=images;
            // await _context.SaveChangesAsync();
            dto.Id=product.Id;
            dto.Images=product.Images.Select(img=>img.Url).ToList();
            return dto;
        }
        catch(Exception e){
            Console.WriteLine(e);
            throw new Exception("Invalid input for products");
        }
        
    }
    public async Task<FilterAttributesResponse> GetProductByFilter(FilterAttributes filterAttributes,int start,int maxSize, bool isAdmin){
        try{
            int minCount=1;
            if(isAdmin)minCount=int.MinValue;
            List<Product> products=await _context.Products.Include(product=>product.Images).Where(p=>p.Price<=filterAttributes.high&&p.Price>=filterAttributes.low)
                                                      .Where(p=>p.Count>=minCount)
                                                      .ToListAsync();
            products=products.Where(p=>p.Name.Contains(filterAttributes.name,StringComparison.OrdinalIgnoreCase)||p.Brand.Contains(filterAttributes.name,StringComparison.OrdinalIgnoreCase)).ToList();
            List<Product> finalProducts=new List<Product>();
            FilterAttributesResponse response=new FilterAttributesResponse();
            
            if(filterAttributes.categories!=null){
                List<string> filterCategories=filterAttributes.categories.ToList();
                foreach(Product product in products){
                    string category=product.Category.ToString().ToLower();
                    foreach(string filCategory in filterCategories){
                        if(category.Contains(filCategory.ToLower())){
                            finalProducts.Add(product);
                            break;
                        }
                    }
                   
                }
            }
            else finalProducts=products;

            SortType type;
            Enum.TryParse(filterAttributes.sortType?.ToUpper(),out type);
            if (type==SortType.PRICE_ASCENDING)finalProducts=finalProducts.OrderBy(product=>product.Price).ToList();
            else if(type==SortType.PRICE_DESCENDING)finalProducts=finalProducts.OrderByDescending(product=>product.Price).ToList();
            
            List<ProductDto> pDto=new List<ProductDto>();
            if(start>=maxSize+start)throw new Exception("Invalid start index");
            if(start+maxSize>=finalProducts.Count){
                maxSize=finalProducts.Count-start;
                response.NextIndex=-1;
            }
            else response.NextIndex=start+maxSize;
            float MaximumPrice=0;
            
            if(start==0){
                response.Total=finalProducts.Count;
                foreach(Product product in finalProducts){
                    if(product.Price>MaximumPrice)MaximumPrice=product.Price;
                }
                response.MaximumPrice=MaximumPrice;
            }
            ProductDto? toAdd;
            finalProducts=finalProducts.GetRange(start,maxSize);
            foreach(Product product in finalProducts ){
                toAdd=ToDto(product);
                if(toAdd!=null)pDto.Add(toAdd);
            }
            response.ProductDtos=pDto;
            return response;
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            throw new Exception();
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
    public async Task<ProductDto?> ModifyProudct(ProductDto product, int id, List<IFormFile> imgFiles){
        Product? match=await _context.Products.Include(p=>p.Images).FirstAsync(p=>p.Id==id);
        if(match==null)throw new Exception();
        match.Name=product.Name??match.Name;
        match.Brand=product.Brand??match.Brand;
        match.Details=product.Details;
        match.Price=product.Price<0?match.Price:product.Price;
        match.Count=product.Count<0?match.Count:product.Count;
        string fileName;
        string extension;
        foreach(IFormFile imgFile in imgFiles){
            fileName=Path.GetRandomFileName();
            extension=Path.GetExtension(imgFile.FileName);
            using(FileStream fs=new FileStream($"Public/Images/{fileName}{extension}",FileMode.Create)){
                await imgFile.CopyToAsync(fs);
            }
            match.Images.Add(new Image{Url=$"{fileName}{extension}"});
        }
        
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
    public async Task<List<ReviewDto>> GetProductReviews(int id,int low,int high, Guid userId){
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
                        IsMine=userId.Equals(_rating.UserId),
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
            return Directory.EnumerateFiles("./Public/Images",$"*",SearchOption.TopDirectoryOnly).ToList<string>();
        });
        presentImages=presentImages.Select(img=>Path.GetFileName(img)).ToList();
        product.Images=product.Images.Where(image=>presentImages.Contains(image.Url)).ToList();
        await _context.SaveChangesAsync();
        return product.Images.Select(img=>img.Url).ToList();
    }
    public async Task DeleteImages(int id, List<string> imgNames){
        Product? product=await _context.Products.Include(p=>p.Images).FirstAsync(p=>p.Id==id);
        if(product==null)return;
        try{
            List<string>? dbImages=product.Images.Select(img=>img.Url).ToList()??new List<string>();
            foreach(string imgName in imgNames){
                if(dbImages.Contains(imgName)){
                      product.Images.RemoveAll(img=>img.ProductId==id&img.Url.Equals(imgName));
                      await Task.Run(()=>{
                        string path="./Public/Images";
                        File.Delete(Path.Join(path,imgName));
                    });
                }
            }
            
            await _context.SaveChangesAsync();
        }
        catch{
            throw new Exception("DeletingFileException");
        }
    }
    public async Task<byte[]?> GetImage(string name){
        byte[] b;
        string path="./Public/Images";
       
        b=await File.ReadAllBytesAsync(Path.Join(path,name));
        return b;
    }
    



}