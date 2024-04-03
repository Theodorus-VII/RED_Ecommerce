using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;
public class Image{
    public int ProudctId{get;set;}
    public Product Product{get;set;}=null!;

    [Required]
    public string Url{get;set;}=string.Empty;

}