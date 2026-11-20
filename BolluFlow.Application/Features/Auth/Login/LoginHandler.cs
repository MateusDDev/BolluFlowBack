using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Infrastructure.Helpers;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Auth.Login;

public class LoginHandler(
    ApplicationDbContext db,
    JwtHelper jwtHelper,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<LoginCommand, ResultOf<UserModel>>
{
    public async Task<ResultOf<UserModel>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = AuthCookie.NormalizeEmail(request.Email);

        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.IsActive, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return ResultOf<UserModel>.Unauthorized(CommonErrors.UserNotFound);

        var token = jwtHelper.GenerateToken(user);

        AuthCookie.Append(httpContextAccessor.HttpContext, token);

        return ResultOf<UserModel>.Success(UserModel.From(user, token));
    }
}
