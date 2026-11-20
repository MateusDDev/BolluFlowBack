using System.Security.Claims;
using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Auth.Me;

public class MeHandler(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<MeQuery, ResultOf<UserModel>>
{
    public async Task<ResultOf<UserModel>> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContextAccessor.HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
            return ResultOf<UserModel>.Unauthorized(new DomainError("USER_NOT_AUTHENTICATED", "Usuário não autenticado"));

        if (!long.TryParse(userIdClaim.Value, out var userId))
            return ResultOf<UserModel>.Unauthorized(new DomainError("INVALID_TOKEN", "Token inválidO"));

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
            return ResultOf<UserModel>.Unauthorized(CommonErrors.UserNotFound);

        var result = UserModel.From(user);
        return ResultOf<UserModel>.Success(result);
    }
}