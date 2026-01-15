using BolluFlow.Application.Features.Auth.Login;
using BolluFlow.Application.Features.Auth.Logout;
using BolluFlow.Application.Features.Auth.Me;
using BolluFlow.Application.Features.Auth.SignUp;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IMediator mediator)
{
    [HttpPost("login")]
    public async Task<ActionResult<UserModel>> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult<UserModel>> SignUp([FromBody] SignUpCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromRoute] LogoutCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserModel>> Me([FromRoute] MeQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        return result.ToActionResult();
    }
}