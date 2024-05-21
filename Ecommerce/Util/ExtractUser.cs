namespace Ecommerce.Utilities;

public static class ExtractUser
{
    public static Guid? GetUserId(this HttpContext context)
    {
        if (context.Items.TryGetValue("UserId", out var userIDobj))
        {
            if (userIDobj is null)
            {
                return null;
            }
            return Guid.Parse(userIDobj?.ToString() ?? "");
        }
        return null;
    }
}