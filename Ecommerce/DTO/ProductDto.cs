using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Ecommerce.Models;
public class ProductDto{
    public int Id{get;set;}
    public string? Name{get;set;}
    public string? Brand{get;set;}
    public string Details{get;set;}="None";
    public int Count{get;set;}=1;
    public string Image{get;set;}="DefaultImage.jpg";
    [EnumDataType(typeof(Category))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Category Category{get;set;}=Category.Other;
    public float Price{get;set;}
}