using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BolluFlow.Infrastructure.Auth;

public class CurrentUser
{
    public long Id { get; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var claim = accessor.HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null)
            throw new UnauthorizedAccessException();

        Id = long.Parse(claim.Value);
    }
}