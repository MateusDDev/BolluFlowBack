using BolluFlow.Application.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BolluFlow.Application.Features.Auth.Logout;

public class LogoutHandler(IHttpContextAccessor httpContextAccessor) : IRequestHandler<LogoutCommand, Result>
{
    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(
            "access_token",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDevelopment,
                SameSite = isDevelopment
                    ? SameSiteMode.Lax
                    : SameSiteMode.None,
            });

        return Task.FromResult(Result.Success());
    }
}