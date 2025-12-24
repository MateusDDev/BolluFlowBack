using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Helpers;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Auth.SignUp;

public class SignUpHandler(ApplicationDbContext db, JwtHelper jwtHelper) : IRequestHandler<SignUpCommand, ResultOf<UserModel>>
{
    public async Task<ResultOf<UserModel>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (existingUser is not null)
            return ResultOf<UserModel>.BadRequest(new DomainError("EMAIL_ALREADY_EXISTS", "Email já cadastrado"));
        
        var user = User.Create(
            request.Name, 
            request.Email, 
            BCrypt.Net.BCrypt.HashPassword(request.Password), 
            request.ProfileImageUrl);
        
        db.Users.Add(user);
        
        await db.SaveChangesAsync(cancellationToken);
        
        var token = jwtHelper.GenerateToken(user);
        
        var result = user.Adapt<UserModel>();
        
        result.Token = token;

        return ResultOf<UserModel>.Success(result);
    }
}