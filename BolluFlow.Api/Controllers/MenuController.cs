using BolluFlow.Application.Features.Menu.CreateMenuOrder;
using BolluFlow.Application.Features.Menu.GetMenu;
using BolluFlow.Application.Features.Menu.ListMenuProducts;
using BolluFlow.Application.Features.Menu.LookupMenuCustomer;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[AllowAnonymous]
[Route("menu")]
public class MenuController(IMediator mediator) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<ActionResult<MenuStoreModel>> GetMenu([FromRoute] string slug, CancellationToken ct)
    {
        var result = await mediator.Send(new GetMenuQuery(slug), ct);
        return result.ToActionResult();
    }

    [HttpGet("{slug}/products")]
    public async Task<ActionResult<List<MenuProductModel>>> GetProducts([FromRoute] string slug, CancellationToken ct)
    {
        var result = await mediator.Send(new ListMenuProductsQuery(slug), ct);
        return result.ToActionResult();
    }

    [HttpGet("{slug}/customers/lookup")]
    public async Task<ActionResult<MenuCustomerLookupModel>> LookupCustomer(
        [FromRoute] string slug,
        [FromQuery] string? whatsapp,
        CancellationToken ct)
    {
        var result = await mediator.Send(new LookupMenuCustomerQuery(slug, whatsapp), ct);
        return result.ToActionResult();
    }

    [HttpPost("{slug}/orders")]
    public async Task<ActionResult<MenuOrderCreatedModel>> CreateOrder(
        [FromRoute] string slug,
        [FromBody] CreateMenuOrderRequestBody body,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateMenuOrderCommand(
                slug,
                body.CustomerName,
                body.CustomerWhatsapp,
                body.CustomerStreet,
                body.CustomerNumber,
                body.CustomerNeighborhood,
                body.CustomerComplement,
                body.IsPickup,
                body.Note,
                body.PaymentMethod,
                body.ChangeFor,
                body.Items),
            ct);

        return result.ToActionResult();
    }
}
