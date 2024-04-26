namespace Ecommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Product
{
    public int Id{get;set;}
    public string Name{get;set;}=null!;
    public string Brand{get;set;}=null!;
    public int Count{get;set;}=1;
    public string Details{get;set;}="None";

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt{get;set;}=DateTime.Now;
    public Category Category{get;set;}=Category.Other;
    public List<Image> Images{get;set;}=new List<Image>();
    public float Price{get;set;}
    public List<Rating> Ratings{get; set;}=new List<Rating>();
}