namespace Ecommerce.Utilities;

public static class ExtractUser
{
    public static Guid? GetUserId(HttpContext context)
    {
        if (context.Items.TryGetValue("UserId", out var userIdObj))
        {
            if (userIdObj is null)
            {
                return null;
            }
            return Guid.Parse(userIdObj?.ToString() ?? "");
        }
        return null;
    }
}