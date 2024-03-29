using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Models;
public interface IProductService{
   public Task<ProductDto?> GetProduct(int id);
    public Task<ProductDto> RegisterProduct(ProductDto dto);
    public Task DeleteProduct(int id);
    // public Task<float> GetAverageRating(int id);
    public Task<List<ProductDto>?> GetProductByFilter(FilterAttributes filterAttributes, int start, int maxSize);
    public Task<ProductDto> ModifyProudct(ProductDto product);  
    public Task BuyProduct(int id);
    public Task<double> GetAverageRating(int id);
    public Task AddRating(int id, int ratingNum, string email);
    public Task ChangeRating(int id, int ratingNum, string email);

}
public class ProductService:IProductService{
    private ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context){
        this._context=context;
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
        Product product=new Product{
            name=dto.Name,
            brand=dto.Brand,
            details=dto.Details,
            category=dto.Category,
            image=dto.Image,
            price=dto.Price

        };
        _context.Products?.Add(product);
        await _context.SaveChangesAsync();
        dto.Id=product.Id;
        return dto;
    }
    public async Task<List<ProductDto>?> GetProductByFilter(FilterAttributes filterAttributes,int start,int maxSize){
        try{
            List<Product> products=await _context.Products.Where(p=>(filterAttributes.category==null||filterAttributes.category==p.category))
                                                      .Where(p=>p.price<=filterAttributes.high&&p.price>=filterAttributes.low)
                                                      .Where(p=>p.name.Contains(filterAttributes.name)||p.brand.Contains(filterAttributes.name))
                                                      .Where(p=>p.count>0)
                                                      .ToListAsync();
                                                      
            List<ProductDto> pDto=new List<ProductDto>();
            if(start>=maxSize)throw new Exception("Invalid start index");
            if(start+maxSize>products.Count)maxSize=products.Count-start;
            products=products.GetRange(start,start+maxSize);
            foreach(Product product in products ){
                pDto.Add(ToDto(product));
            }
            return pDto;
        }
        catch{
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
        double average=0;
        foreach(Rating rating in ratings)average+=rating.rating;
        average/=ratings.Count;
        return average;

    }
    public async Task<ProductDto> ModifyProudct(ProductDto product){
        Product match=await _context.Products.FirstAsync(p=>p.Id==product.Id);
        match.name=product.Name;
        match.brand=product.Brand;
        match.details=product.Details;
        match.image=product.Image;
        match.price=product.Price;
        await _context.SaveChangesAsync();
        return product;
    }
    public async Task BuyProduct(int id){
        Product? product=await _context.Products.FindAsync(id);
        if(product!=null)product.count-=1;
        await _context.SaveChangesAsync();
    }
    public async Task AddRating(int id,int ratingNum, string email){
        Product product=await _context.Products.FirstAsync(p=>p.Id==id);
        product?.ratings?.Add(new Rating{
            rating=ratingNum,
            ProductId=id,
            Email=email,
        });
        await _context.SaveChangesAsync();
    }
    public async Task ChangeRating(int id,int ratingNum, string email){
        Rating rating=await _context.Ratings.FirstAsync(r=>r.ProductId==id&&r.Email.Equals(email));
        rating.rating=ratingNum;
        await _context.SaveChangesAsync();
    }


}