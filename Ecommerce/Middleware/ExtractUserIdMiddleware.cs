using System.Security.Claims;

namespace Ecommerce.Utilities;

public class ExtractUserIdMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier
        );

        if (userIdClaim != null)
        {
            Guid userId = Guid.Parse(userIdClaim.Value);
            context.Items.Add("UserId", userId);
        }
        // extract the user Id from the claim.
        await next(context);
    }
}
