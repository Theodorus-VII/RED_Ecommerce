namespace Ecommerce.Utilities;


public interface IServiceResponse<T>
{
    int StatusCode { get; set; }
    public T? Data { get; set; }
    public ErrorResponse Error { get; set; }
    public bool IsSuccess { get; set; }
}
public class ServiceResponse<T> : IServiceResponse<T>
{
    public int StatusCode { get; set; }
    public T? Data { get; set; } 
    public ErrorResponse Error { get; set; } = new ErrorResponse();
    public bool IsSuccess { get; set; }
}

public class ErrorResponse
{
    public int ErrorCode { get; set; } = 500;
    public string ErrorDescription { get; set; } = "Server Error";
}