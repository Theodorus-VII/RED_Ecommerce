using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Models;
public interface IProductService{
   public Task<ProductDto> GetProduct(int id);
    public Task<ProductDto> RegisterProduct(ProductDto dto);
    public Task DeleteProduct(int id);
    // public Task<float> GetAverageRating(int id);
    public Task<List<ProductDto>> GetProductByFilter(FilterAttributes filterAttributes);
    // public Task<ProductDto> UpdateProudct(ProductDto product);  

}
public class ProductService:IProductService{
    private ApplicationDbContext _context;
    public ProductService(ApplicationDbContext context){
        this._context=context;
    }
    public  async Task<ProductDto>  GetProduct(int id){
       Product? match=await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);
       return ToDto(match);
    }
    public ProductDto ToDto(Product? myProduct){
            if(myProduct==null){
                return new ProductDto();
            }
            ProductDto myDto=new ProductDto();
            myDto.Id=myProduct.Id;
            myDto.Name=myProduct.name??string.Empty;
            myDto.Brand=myProduct.brand??string.Empty;
            myDto.Details=myProduct.details??"None";
            myDto.Category=myProduct.category;
            myDto.Price=myProduct.price;
            myDto.Image=myProduct.image??"DefaultImage.jpeg";
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
    public async Task<List<ProductDto>> GetProductByFilter(FilterAttributes filterAttributes){
        List<Product> products=await _context.Products.Where(p=>(filterAttributes.category==null||filterAttributes.category==p.category))
                                                      .Where(p=>p.price<=filterAttributes.high&&p.price>=filterAttributes.low)
                                                      .Where(p=>p.name.Contains(filterAttributes.name)||p.brand.Contains(filterAttributes.name))
                                                      .ToListAsync();
                                                      
        List<ProductDto> pDto=new List<ProductDto>();
        foreach(Product product in products ){
            pDto.Add(ToDto(product));
        }
        return pDto;
    }
     public async Task DeleteProduct(int id){
        Product product=await _context.Products.SingleAsync(p=>p.Id==id);
        _context.Remove(product);
        await _context.SaveChangesAsync();
    }
    // public async Task<int> GetAverageRating(int id){

    // }


}