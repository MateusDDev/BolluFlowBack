using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Auth.Login;
using BolluFlow.Application.Features.Auth.Me;
using BolluFlow.Application.Features.Auth.SignUp;
using BolluFlow.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IMediator mediator)
{
    [HttpPost("login")]
    public async Task<ResultOf<UserModel>> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }

    [HttpPost("sign-up")]
    public async Task<ResultOf<UserModel>> SignUp([FromBody] SignUpCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }

    [HttpGet("me")]
    public async Task<ResultOf<UserModel>> Me([FromRoute] MeQuery query, CancellationToken ct)
    {
        return await mediator.Send(query, ct);
    }
}