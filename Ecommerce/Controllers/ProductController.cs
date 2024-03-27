using Microsoft.AspNetCore.Mvc;
namespace Ecommerce.Models;
[ApiController]
[Route("/product")]
public class ProductController:ControllerBase{
    private IProductService _services;
    public ProductController(IProductService services){
        _services=services;
    }
    [HttpGet("{id}")]
    public async  Task<ActionResult<ProductDto>> GetProduct(int id){
        ProductDto? result;
        try{
            result= await _services.GetProduct(id);
            if(result is null)return NotFound();
            return Ok(result);
        }
        catch(Exception e){
            
        }
        return NotFound();
        
    }
    [HttpPost]
    public async Task<ActionResult> PostProduct([FromBody]ProductDto dto){
        ProductDto myDto=await _services.RegisterProduct(dto);
        return Created(string.Empty,myDto);
    }
     [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveProduct(int id){
        await _services.DeleteProduct(id);
        return NoContent();
    }


}