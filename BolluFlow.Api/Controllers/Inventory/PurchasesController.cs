using BolluFlow.Application.Features.Inventory.CreatePurchase;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Inventory;

[ApiController]
[Route("api/inventory/purchases")]
[Authorize]
public class PurchasesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreatePurchaseCommand command, CancellationToken ct)
        => (await mediator.Send(command, ct)).ToActionResult();
}
