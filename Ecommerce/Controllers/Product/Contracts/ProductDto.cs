using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ecommerce.Models;
namespace Ecommerce.Controllers.Contracts;
public class ProductDto{
    public int Id{get;set;}
    public string? Name{get;set;}
    public string? Brand{get;set;}
    public string Details{get;set;}="None";
    public int Count{get;set;}=-1;
    public List<string> Images{get;set;}=new List<string>();
    [EnumDataType(typeof(Category))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Category Category{get;set;}=Category.Other;
    public float Price{get;set;}=-1;
}