using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
// using Ecommerce.Models;
using Ecommerce.Controllers.Contracts;
using Ecommerce.Models;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Utilities;
using System.Net;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Authorize]
[Route("/product")]
public class ProductController : ControllerBase
{
    private ILogger _logger;
    private IProductService _services;
    private IUserAccountService _userService;
    public ProductController(IProductService services, IUserAccountService userService, ILogger<ProductController> logger)
    {
        _logger = logger;
        _services = services;
        _userService = userService;
    }
/// <summary>
/// Search for products
/// </summary>
/// <param name="categories">List of categories to be included in search results.</param>
/// <param name="high">Higher limit of product price range. Products above this price aren't included in results</param>
/// <param name="low">Lower limit of product price range. Product this aren't shown</param>
/// <param name="name">The search term that is to be used while searching products.</param>
/// <param name="start">The start index of results. Results of only this index and above are shown</param>
/// <param name="maxSize">The maximum size of the products to be fetched. Number of results can't exceed this number</param>
/// <returns> List of filtered product with their details </returns>
/// <response code="200">
/// Successfully  fetched results
///     A an object contatining a list of results
///     {
///         "productDtos":[{
///              "id": 14,
                /// "name": "Name",
                /// "brand": "cat",
                /// "details": "None",
                /// "count": 6000,
                /// "images": [
                ///     "img2.jpg"
                /// ],
                /// "category": "HomeCleaning",
                /// "price": 1200
///          },...]
///     }
/// </response>
    [HttpGet]
    public async Task<ActionResult<FilterAttributesResponse>> GetFilteredProducts(string? Categories = "", string? name = "", int start = 0, int maxSize = 10, int low = 0, int high = int.MaxValue)
    {
        string[]? categories = Categories?.Split(",");
        // List<Category> catList=new List<Category>();
        // Category toAdd;
        // foreach(string strCategory in categories){
        //     if(Enum.TryParse<Category>(strCategory,out toAdd))catList.Add(toAdd);
        //     Console.WriteLine(toAdd);
        // }
        FilterAttributes filter = new FilterAttributes { categories = categories, name = name ?? "", low = low, high = high };
        FilterAttributesResponse? products = await _services.GetProductByFilter(filter, start, maxSize);
        // if(products==null)return BadRequest("Wrong parameter or filter property values");
        // return Ok(products);


        return Ok(products);
    }
/// <summary>
/// Fetch details of a particular product through its id
/// </summary>
/// <param name="id">Product ID</param>
/// <returns>
/// <returns> Endpoint returns specific product details </returns>
/// <response code="200">
/// Successfully  fetched product
///    
///     A an object contatining the product details
///     {
///         "id": 14,
        /// "name": "Name",
        /// "brand": "cat",
        /// "details": "product characteristics description",
        /// "count": 6000,
        /// "images": [
        ///     "img2.jpg"
        /// ],
        /// "category": "HomeCleaning",
        /// "price": 1200
///     }
/// </response>
/// <response code="404">
///     Product is not found
/// </response>
/// <response code="500">
///     A sever error has occured
/// </response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        ProductDto? result;
        try
        {
            result = await _services.GetProduct(id);
            if (result is null) return NotFound();
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Problem(statusCode:500,title:"Internal server error");
        }
        
    }
/// <summary>
/// Add a new product
/// </summary>
/// <param name="dto">This is an object included in the body with the following format
/// {
///     "name":"Jordan Monogram bag",  //product name/model
///     "brand":"Nike",  //Brand name of the company that manufactured the product,
///     "details":"A very big comfortable bag",  //A description of the characteristics of the product
///     "count":60, // An INTEGER number that specifies the number of products available for purchase
///     "price":120, //A FLOAT number that specifies the price of the product
///     "images":["img2.jpg"],  //list of product images
///     "cateogry":"Fashion" //Category in which the product is included
/// }
/// </param>
/// <returns> Product details</returns>
/// <response code="201">
/// Product has been created successfully
/// An object contatining the product details is returned
///     {
///         "id": 12,
        /// "name": "Jordan Monogram bag",
        /// "brand": "Nike",
        /// "details": "A very big comfortable bag",
        /// "count": 60,
        /// "images": [
        ///     "img2.jpg"
        /// ],
        /// "category": "Fashion",
        /// "price": 120
///     }
/// </response>
/// <response code="400">
///     Invalid request format
/// </response>
    [HttpPost]
    // [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> PostProduct([FromBody] ProductDto dto)
    {
        try
        {
            ProductDto myDto = await _services.RegisterProduct(dto);
            return Created(string.Empty, myDto);
        }
        catch
        {
            return BadRequest("Invlaid request! Make sure you have entered everything");
        }

    }
/// <summary>
/// Delete product
/// </summary>
/// <param name="id">id of the product to be deleted</param>
/// <response code="204">
///  Product is deleted successully. No content.
/// </response>
/// <response code="404">
/// Product to be deleted is not found
/// </response>
     [HttpDelete("{id}")]
    //  [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> RemoveProduct(int id)
    {
        try
        {
            await _services.DeleteProduct(id);
            return NoContent();
        }
        catch
        {
            return NotFound("Product doesn't exist");
        }


    }
/// <summary>
/// Update product details
/// </summary>
/// <param name="dto">This is an object included in the body with the following format.
/// {
///     "name":"Jordan Monogram bag",  //product name/model
///     "brand":"Nike",  //Brand name of the company that manufactured the product,
///     "details":"A very big comfortable bag",  //A description of the characteristics of the product
///     "count":59, // An INTEGER number that specifies the number of products available for purchase
///     "price":120, //A FLOAT number that specifies the price of the product
///     "images":["img2.jpg"],  //list of product images
///     "cateogry":"Fashion" //Category in which the product is included
/// }
/// </param>
/// <param name="id">ID of the product to be updated</param>
/// <returns> Updated product details are returned </returns>
/// <response code="200">
/// Product has been created successfully
///     A an object contatining the updated product details:
///     {
///         "id": 12,
        /// "name": "Jordan Monogram bag",
        /// "brand": "Nike",
        /// "details": "A very big comfortable bag",
        /// "count": 59,
        /// "images": [
        ///     "img2.jpg"
        /// ],
        /// "category": "Fashion",
        /// "price": 120
///     }
/// </response>
/// <response code="400">
///     Invalid request format
/// </response>
/// <response code="404">
///     Product to be updated is not found
/// </response>
/// <response code="500"> Server error has occured </response>

    [HttpPatch("{id}")]
    // [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<ProductDto>> ChangeProduct([FromBody] ProductDto dto, int id)
    {
        try
        {
            ProductDto resDto = await _services.ModifyProudct(dto, id);
            if (resDto != null) return Ok(resDto);
            return NotFound();
        }
        catch (InvalidDataException)
        {
            return BadRequest("Invalid values for data");
        }
        catch (Exception)
        {
            return NotFound("Product doesn't exist");
        catch(Exception){
            return Problem(statusCode:500,detail:"Product doesn't exist");
        }
    }
/// <summary>
///     Get average rating of the product
/// </summary>
/// <param name="id">Id of the </param>
/// <returns> Average rating of a product is returned </returns>
/// <response code="200">Average rating of the product. Returns -1 if no rating is available</response>
/// <response code="404">Product doesn't exist</response>
    [HttpGet("{id}/rating")]
    public async Task<ActionResult<double>> GetRating(int id)
    {
        try
        {
            double average = await _services.GetAverageRating(id);
            return Ok(average);
        }
        catch
        {
            return NotFound();
        }
    }
/// <summary>
///     Add or update rating and review to a product
/// </summary>
/// <param name="id"></param>
/// <param name="ratingDto">RatingDto Object in body with form:
/// {
///     "rating":5, //A number from 1 to 10 given as a rating out of 10 for the product
///     "review": "A mediocre product with few good qualities" //Review for the product. This is optional
/// }
/// </param>
/// <response code="204">No content. Rating added successfully</response>
/// <response code="401">Credentials not found. Authentication problem</response>
/// <response code="404">Product not found</response>
/// <response code="500">An error has occurred in the server</response>
    [HttpPut("{id}/rating")]
    public async Task<ActionResult> AddRating(int id, [FromBody] RatingDto ratingDto)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("Invalid request provided");
            return Unauthorized("Credentials not found");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);
        // User? user = await _userService.GetUserById(userId);
        // if(user==null)return NotFound("This user doesn't exist");
        try{
            await _services.AddRating(id,ratingDto,userId);
            return NoContent();
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(exception);
            return Problem(statusCode:500,detail:"An error has occurred in the server");
        }
        catch(Exception){
            return NotFound("Product doesn't exist");
        }
        
    }
/// <summary>
/// Delete a rating for a product
/// </summary>
/// <param name="id"></param>
/// <response code="204">No content</response>
/// <response code="401">Credentials not found<>
/// <response code="404">Rating doesn't exist</response>
    [HttpDelete("{id}/rating")]
    public async Task<ActionResult> DeleteRating(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // return error if the user Id isn't in the token claims.
        _logger.LogInformation($"{userIdClaim}");
        if (userIdClaim == null)
        {
            _logger.LogError("Invalid request");
            return Unauthorized("Invalid request");
        }
        // extract the user Id from the claim.
        Guid userId = Guid.Parse(userIdClaim.Value);
        // User? user = await _userService.GetUserById(userId);
        // if(user==null)return NotFound("This user doesn't exist");
        try{
            await _services.DeleteRating(id,userId);
            return NoContent();
        }
        catch
        {
            return NotFound("Rating doesn't exist");
        }


    }
/// <summary>
///  Get reviews for a product
/// </summary>
/// <param name="id">ID of the product</param>
/// <param name="lowRating">The lower limit of the rating range for the product whose reviews are to be fetched</param>
/// <param name="highRating">The higher limit of the rating range for the product whose reviews are to be fetched</param>
/// <response code="200"> A list of reviews </response>
/// <response code="500"> Internal server has occurred </response>
    [HttpGet("{id}/review")]
    public async Task<ActionResult<List<RatingDto>>> GetReviews(int id, [FromQuery] int lowRating = 0, [FromQuery] int highRating = 10)
    {
        try
        {
            List<ReviewDto> reviews;
            reviews = await _services.GetProductReviews(id, lowRating, highRating);
            if (reviews == null) return NotFound("No Rating/Review Found");
            return Ok(reviews);
        }
        catch
        {
            return StatusCode(
                500,
                "Server Error"
            );
        }
        
    }
/// <summary>
/// Returns a list of image urls for a particular product after refreshing its list by checking images directory
/// </summary>
/// <param name="id"></param>
/// <returns> A list of image urls: ["PID1_1.jpeg", "PID1_2.jpeg"] </returns>
/// <response code="200"> A list of image urls </response>
/// <response code="500"> Internal Server error has occurred </response>
  
    [HttpGet("{id}/image")]
    public async Task<ActionResult<List<string>>> GetRefreshedImageList(int id){
        try{
            List<string>? images= await _services.RefreshImages(id)??new List<string>();;
            return Ok(images);
        }
        catch
        {
            return StatusCode(
                500,
                "Some internal error occured while processing your request"
            );
        }

    }
    // [HttpGet("{id}/image/{picId}")]
    // public async Task<ActionResult?> GetImage(int id, string picId){
    //     byte[]? imgBytes;
    //     string name=$"PID{id}_{picId}.jpg";
    //     try{
    //         imgBytes=await _services.GetImage(name);
    //         if(imgBytes==null)imgBytes=await System.IO.File.ReadAllBytesAsync("./Public/Images/DefaultImage.jpg");
    //         return File(imgBytes,"image/jpeg");
    //     }
    //     catch{
    //         imgBytes=await System.IO.File.ReadAllBytesAsync("./Public/Images/DefaultImage.jpg");
    //         return File(imgBytes,"image/jpeg");
    //     }
    // }
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <param name="images"></param>
/// <response code="204"> No content </response>
/// <response code"="500"> Internal Server error has occurred </response>
    [HttpDelete("{id}/image")]
    public async Task<ActionResult> DeleteImages(int id, string images)
    {
        List<string> imgNames = new List<string>(images.Split(","));
        try
        {
            await _services.DeleteImages(id, imgNames);
            return NoContent();
        }
        catch
        {
            return NotFound("Image doesn't exist");
        }
    }
}