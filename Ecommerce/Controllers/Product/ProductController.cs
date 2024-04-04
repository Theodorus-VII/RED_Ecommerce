using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
// using Ecommerce.Models;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Org.BouncyCastle.Asn1.Cmp;


[ApiController]
[Authorize]
[Route("/product")]
public class ProductController:ControllerBase{
    private ILogger _logger;
    private IProductService _services;
    private IUserAccountService _userService;
    public ProductController(IProductService services, IUserAccountService userService,ILogger<ProductController> logger){
        _logger=logger;
        _services=services;
        _userService=userService;
    }
    [HttpGet]
    public async Task<ActionResult<FilterAttributesResponse>> GetFilteredProducts( [FromBody] FilterAttributes filter,[FromQuery] int start=0,[FromQuery]int maxSize=10){
        FilterAttributesResponse? products=await _services.GetProductByFilter(filter,start,maxSize);
        if(products==null)return BadRequest("Wrong parameter or filter property values");
        return Ok(products);
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
            _logger.LogError(e.Message);
            return Problem(statusCode:500,title:"Internal server error");
        }
        
    }
    [HttpPost]
    // [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> PostProduct([FromBody]ProductDto dto){
        try{
            ProductDto myDto=await _services.RegisterProduct(dto);
            return Created(string.Empty,myDto);
        }
        catch{
            return BadRequest("Invlaid request! Make sure you have entered everything");
        }
        
    }
     [HttpDelete("{id}")]
    //  [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> RemoveProduct(int id){
        try{
            await _services.DeleteProduct(id);
            return NoContent();
        }
        catch{
            return NotFound("Product doesn't exist");
        }
        
        
    }
    [HttpPatch("{id}")]
    // [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<ProductDto>> ChangeProduct([FromBody]ProductDto dto,int id){
        try{
            ProductDto resDto=await _services.ModifyProudct(dto,id);
            if(resDto!=null)return Ok(resDto);
            return NotFound();
        }
        catch(InvalidDataException){
            return BadRequest("Invalid values for data");
        }
        catch(Exception){
            return NotFound("Product doesn't exist");
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
    public async Task<ActionResult> AddRating(int id,[FromBody] RatingDto ratingDto){
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("Invalid request provided");
            return BadRequest("Invalid request");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);
        // User? user = await _userService.GetUserById(userId);
        // if(user==null)return NotFound("This user doesn't exist");
        await _services.AddRating(id,ratingDto,userId);
        return Created(string.Empty,ratingDto);
    }
    [HttpDelete("{id}/rating")]
    public async Task<ActionResult> DeleteRating(int id){
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("Invalid request");
            return BadRequest("Invalid request");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);
        // User? user = await _userService.GetUserById(userId);
        // if(user==null)return NotFound("This user doesn't exist");
        try{
            await _services.DeleteRating(id,userId);
            return Ok("Rating Deleted Successfully");
        }
        catch{
            return NotFound("Rating doesn't exist");
        }
        
        
    }
    [HttpGet("{id}/reviews")]
    public async Task<ActionResult<List<RatingDto>>> GetReviews(int id,[FromQuery]int lowRating=0,[FromQuery]int highRating=10){
        try{
            List<ReviewDto> reviews; 
            reviews=await _services.GetProductReviews(id,lowRating,highRating);
            if(reviews==null)return NotFound("No Rating/Review Found");
            return Ok(reviews);
        }
        catch{
            return Problem(statusCode:500,title:"Server Error");
        }
        
    }
    [HttpGet("{id}/images")]
    public async Task<ActionResult<List<string>>> GetRefreshedImageList(int id){
        try{
            List<string>? images= await _services.RefreshImages(id)??new List<string>();;
            return images;
        }
        catch{
            return Problem(statusCode:500,detail:"Some internal error occured while processing your request");
        }

    }
    [HttpGet("{id}/images/{url}")]
    public async Task<ActionResult?> GetImage(int id, string picId){
        byte[]? imgBytes;
        string name=$"PID{id}_picId.jpg";
        try{
            imgBytes=await _services.GetImage(name);
            if(imgBytes==null)imgBytes=await System.IO.File.ReadAllBytesAsync("./Public/Images/DefaultImage.jpg");
            return File(imgBytes,"image/jpeg");
        }
        catch{
            return Problem(statusCode:500,detail:"Server error while processing request");
        }
    }



}