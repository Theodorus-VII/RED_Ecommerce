using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Ecommerce.Models;
public class ProductDto{
    public int Id{get;set;}
    public string Name{get;set;}=string.Empty;
    public string Brand{get;set;}=string.Empty;
    public string Details{get;set;}="None";
    public string Image{get;set;}="DefaultImage.jpg";
    [EnumDataType(typeof(Category))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Category? Category{get;set;}
    public float Price{get;set;}
}