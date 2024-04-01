using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ecommerce.Models;
namespace Ecommerce.Controllers.Contracts;
public class FilterAttributes{
    public float low{get;set;}=0;
    public float high{get;set;}=float.MaxValue;
    [EnumDataType(typeof(Category))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Category? category{get;set;}=null;
    public string name{get;set;}=string.Empty;
}