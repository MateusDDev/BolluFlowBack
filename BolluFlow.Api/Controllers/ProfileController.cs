using BolluFlow.Application.Features.Profile.GetProfile;
using BolluFlow.Application.Features.Profile.UpdateProfile;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Authorize]
[Route("profile")]
public class ProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProfileModel>> GetProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new GetProfileQuery(), ct);

        return result.ToActionResult();
    }

    [HttpPut]
    public async Task<ActionResult<ProfileModel>> UpdateProfile([FromBody] UpdateProfileCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateProfileCommand(body), ct);

        return result.ToActionResult();
    }
}
