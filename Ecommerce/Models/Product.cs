namespace Ecommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Product
{
    public int Id{get;set;}
    public string Name{get;set;}=null!;
    public string Brand{get;set;}=null!;
    public int Count{get;set;}=1;
    public string Details{get;set;}="None";
    public Category Category{get;set;}=Category.Other;
    public List<Image> Images{get;set;}=new List<Image>();
    public float Price{get;set;}
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? CreatedAt{get;set;}
    public List<Rating> Ratings{get; set;}=new List<Rating>();
}