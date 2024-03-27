namespace Ecommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Product
{
    public int Id{get;set;}
    public string? name{get;set;}
    public string? brand{get;set;}
    public int count{get;set;}=1;
    public string details{get;set;}="None";
    [Column(TypeName ="varchar(10)")]
    public Category? category{get;set;}
    public string? image{get;set;}="defaultImage.jpg";
    public float price{get;set;}
    public List<Rating>? ratings{get;}
}