using BolluFlow.Application.Features.Orders.OrderItems.CreateOrderItem;
using BolluFlow.Application.Features.Orders.OrderItems.DeleteOrderItem;
using BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;
using BolluFlow.Application.Features.Orders.OrderItems.UpdateOrderItem;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("orders/{orderId:long}/[controller]")]
[Authorize]
public class OrderItemsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrderItem(
        [FromRoute] long orderId,
        [FromBody] List<CreateOrderItemCommandBody> body,
        CancellationToken ct)
    {
        var command = new CreateOrderItemCommand(orderId, body);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpDelete("{itemId:long}")]
    public async Task<IActionResult> DeleteItem([FromRoute] long orderId, [FromRoute] long itemId, CancellationToken ct)
    {
        var command = new DeleteOrderItemCommand(orderId, itemId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPut("{itemId:long}")]
    public async Task<IActionResult> UpdateItem(
        [FromRoute] long orderId,
        [FromRoute] long itemId,
        [FromBody] UpdateOrderItemCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateOrderItemCommand(orderId, itemId, body);

        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> ListItems([FromRoute] long orderId, CancellationToken ct)
    {
        var query = new ListOrderItemsQuery(orderId);
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }
}