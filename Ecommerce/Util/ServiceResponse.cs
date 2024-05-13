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
   
    /// <summary>
    ///     Creates a new instance of <see cref="ServiceResponse{T}"/> with
    ///     success status that signifies a successfull operation.
    /// </summary>
    /// <typeparamref name="T">
    ///     The type of the data to be included in the response.
    /// </typeparamref>
    /// <param name="data">
    ///     The data to be included in the response. 
    ///     Has a type of <typeparamref name="T"/>.
    /// </param>
    /// <param name="statusCode">
    ///     The HTTP status code to be included in the response.
    /// </param>/// 
    /// <returns>
    ///     A new instance of <see cref="ServiceResponse{T}"/> with 
    ///     success status and the provided data and status code.
    /// </returns>
    public static IServiceResponse<T> SuccessResponse(T data, int statusCode)
    {
        return new ServiceResponse<T>()
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    /// <summary>
    ///     Creates a new instance of <see cref="ServiceResponse{T}"/> with
    ///     failure status that signifies an unsuccessful operation.
    /// </summary>
    /// <remarks> 
    /// <typeparamref name="T"/>
    ///     The type of the data to be included in the response.
    /// <param name="statusCode">
    ///     The HTTP status code to be included in the response.
    /// </param>
    /// <param name="errorDescription">
    ///     The description of the error to be included in the response.
    /// </param>
    /// </remarks>
    /// <returns>
    ///     A new instance of <see cref="ServiceResponse{T}"/> with
    ///     failure status, the provided status code, and the specified error description.
    /// </returns>
    public static IServiceResponse<T> FailResponse(int statusCode, string errorDescription)
    {
        return new ServiceResponse<T>()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Error = new ErrorResponse()
            {
                ErrorCode = statusCode,
                ErrorDescription = errorDescription
            }
        };
    }
}


public class ErrorResponse
{
    public int ErrorCode { get; set; } = 500;
    public string ErrorDescription { get; set; } = "Server Error";
}