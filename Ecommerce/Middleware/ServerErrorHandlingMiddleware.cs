namespace Ecommerce.Middleware;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        ILogger<ErrorHandlingMiddleware> logger
    )
    {
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unhandled exception has occurred");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse =  @"An unexpected error ocurred. Please try again later or contact the system administrator.";

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}