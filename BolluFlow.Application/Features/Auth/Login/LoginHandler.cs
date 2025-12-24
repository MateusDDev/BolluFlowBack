using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Helpers;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Auth.Login;

public class LoginHandler(ApplicationDbContext db, JwtHelper jwtHelper) : IRequestHandler<LoginCommand, ResultOf<UserModel>>
{
    public async Task<ResultOf<UserModel>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return ResultOf<UserModel>.Unauthorized(CommonErrors.UserNotFound);

        var token = jwtHelper.GenerateToken(user);
        
        var result = user.Adapt<UserModel>();
        
        result.Token = token;
        
        return ResultOf<UserModel>.Success(result);
    }
}