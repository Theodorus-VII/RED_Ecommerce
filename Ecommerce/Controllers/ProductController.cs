using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Ecommerce.Models;
[ApiController]
[Route("/product")]
public class ProductController:ControllerBase{
    private ILogger _logger;
    private IProductService _services;
    private IUserAccountService _userService;
    public ProductController(IProductService services, IUserAccountService userService,ILogger logger){
        _logger=logger;
        _services=services;
        _userService=userService;
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
            Console.WriteLine(e.Message);
        }
        return NotFound();
        
    }
    [HttpPost]
    public async Task<ActionResult> PostProduct([FromBody]ProductDto dto){
        try{
            ProductDto myDto=await _services.RegisterProduct(dto);
            return Created(string.Empty,myDto);
        }
        catch{
            return BadRequest();
        }
        
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
            return NotFound();
        }
    }
    [HttpPut("{id}/rating")]
    public async Task<ActionResult> AddRating(int id,[FromQuery] int rating){
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("User Id claim not found within the token provided");
            return BadRequest("Invalid user");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);
        User? user = await _userService.GetUserById(userId);
        if(user==null)return NotFound("This user doesn't exist");
        await _services.AddRating(id,rating,user.Email);
        return Created(string.Empty,rating);
    }
    [HttpPatch("{id}/rating")]
    public async Task<ActionResult> ChangeRating(int id,[FromQuery] int rating){
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("User Id claim not found within the token provided");
            return BadRequest("Invalid user");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);
        User? user = await _userService.GetUserById(userId);
        if(user==null)return NotFound("This user doesn't exist");
        await _services.ChangeRating(id,rating,user.Email);
        return Ok("Rating changed successfuly");
    }
    



}