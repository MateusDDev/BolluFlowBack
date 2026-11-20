using System.Net.Mail;
using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Helpers;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Auth.SignUp;

public class SignUpHandler(
    ApplicationDbContext db,
    JwtHelper jwtHelper,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SignUpCommand, ResultOf<UserModel>>
{
    private const int MinimumPasswordLength = 8;

    public async Task<ResultOf<UserModel>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        var email = AuthCookie.NormalizeEmail(request.Email);

        if (name.Length < 2)
            return ResultOf<UserModel>.BadRequest(new DomainError("SIGN_UP_NAME_INVALID", "Informe o nome do negócio"));

        if (!MailAddress.TryCreate(email, out _))
            return ResultOf<UserModel>.BadRequest(new DomainError("SIGN_UP_EMAIL_INVALID", "Email inválido"));

        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < MinimumPasswordLength)
            return ResultOf<UserModel>.BadRequest(new DomainError("SIGN_UP_PASSWORD_INVALID", $"A senha precisa ter pelo menos {MinimumPasswordLength} caracteres"));

        var emailInUse = await db.Users.AnyAsync(x => x.Email.ToLower() == email, cancellationToken);

        if (emailInUse)
            return ResultOf<UserModel>.Conflict(new DomainError("EMAIL_ALREADY_EXISTS", "Este email já tem uma conta"));

        var user = User.Create(
            name,
            email,
            BCrypt.Net.BCrypt.HashPassword(request.Password),
            request.ProfileImageUrl);

        db.Users.Add(user);

        await db.SaveChangesAsync(cancellationToken);

        var token = jwtHelper.GenerateToken(user);

        AuthCookie.Append(httpContextAccessor.HttpContext, token);

        return ResultOf<UserModel>.Success(UserModel.From(user, token));
    }
}
