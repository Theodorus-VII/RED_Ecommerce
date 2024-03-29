namespace Ecommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Product
{
    public int Id{get;set;}
    public string name{get;set;}=null!;
    public string brand{get;set;}=null!;
    public int count{get;set;}=1;
    public string details{get;set;}="None";
    public Category category{get;set;}=Category.Other;
    public string? image{get;set;}="DefaultImage.jpg";
    public float price{get;set;}
    public List<Rating> ratings{get; set;}=new List<Rating>();
}