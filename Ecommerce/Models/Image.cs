using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;
public class Image
{

    public string Url { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

}