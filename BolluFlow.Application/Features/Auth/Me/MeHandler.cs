using System.Security.Claims;
using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
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
            return ResultOf<UserModel>.Unauthorized("Usuário não autenticado");

        if (!long.TryParse(userIdClaim.Value, out var userId))
            return ResultOf<UserModel>.Unauthorized("Token inválido");

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
            return ResultOf<UserModel>.Unauthorized("Usuário não encontrado");

        var result = user.Adapt<UserModel>();
        
        result.Token = "";

        return ResultOf<UserModel>.Success(result);
    }
}