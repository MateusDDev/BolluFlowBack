using BolluFlow.Application.Features.Onboarding.CompleteOnboarding;
using BolluFlow.Application.Features.Onboarding.GetOnboardingProgress;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Authorize]
[Route("onboarding")]
public class OnboardingController(IMediator mediator) : ControllerBase
{
    [HttpPost("complete")]
    public async Task<ActionResult<UserModel>> Complete([FromBody] CompleteOnboardingCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpGet("progress")]
    public async Task<ActionResult<GetOnboardingProgressResult>> GetProgress(CancellationToken ct)
    {
        var result = await mediator.Send(new GetOnboardingProgressQuery(), ct);

        return result.ToActionResult();
    }
}
