using Microsoft.AspNetCore.Http;

namespace BolluFlow.Application.Services;

public static class AuthCookie
{
    public const string Name = "access_token";

    public static void Append(HttpContext? httpContext, string token)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        httpContext?.Response.Cookies.Append(
            Name,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDevelopment,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(4)
            });
    }

    public static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToLowerInvariant();
    }
}
