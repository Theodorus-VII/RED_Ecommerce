using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> ChangeProduct([FromBody] ProductDto dto){
        try{
            ProductDto resDto=await _services.ModifyProudct(dto);
            if(resDto!=null)return Ok(resDto);
            return NotFound();
        }
        catch{
            return Problem(statusCode:500);
        }
    }
    [HttpGet("{id}/rating")]
    public async Task<ActionResult<double>>  GetRating(int id){
        try{
            double average=await _services.GetAverageRating(id);
            return Ok(average);
        }
        catch{
            return Problem(statusCode:500);
        }
        

    }


}