using BolluFlow.Application.Features.Supplies.CreateSupply;
using BolluFlow.Application.Features.Supplies.DeleteSupply;
using BolluFlow.Application.Features.Supplies.GetSupplyDetails;
using BolluFlow.Application.Features.Supplies.ListSupplies;
using BolluFlow.Application.Features.Supplies.UpdateSupply;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Supplies;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SuppliesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SupplyModel>>> List(CancellationToken ct)
    {
        var result = await mediator.Send(new ListSuppliesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<SupplyModel>> GetById(long id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSupplyDetailsQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<CreateSupplyResult>> Create([FromBody] CreateSupplyCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<SupplyModel>> Update(long id, [FromBody] UpdateSupplyCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateSupplyCommand(id, body), ct);
        return result.ToActionResult();
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteSupplyCommand(id), ct);
        return result.ToActionResult();
    }
}
