namespace Ecommerce.Services;

public class EmailDto
{
    public string Subject {get; set;} = "";
    public string Recipient {get; set;} = "";
    public string Message {get; set;} = "";
}