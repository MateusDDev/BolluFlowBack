using BolluFlow.Application.Features.Orders.OrderItems.CreateOrderItems;
using BolluFlow.Application.Features.Orders.OrderItems.DeleteOrderItem;
using BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;
using BolluFlow.Application.Features.Orders.OrderItems.UpdateOrderItem;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("orders/{orderId:long}/items")]
[Authorize]
public class OrderItemsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<List<OrderItemModel>>> CreateOrderItems(
        [FromRoute] long orderId,
        [FromBody] List<CreateOrderItemCommandBody> body,
        CancellationToken ct)
    {
        var command = new CreateOrderItemsCommand(orderId, body);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpDelete("{itemId:long}")]
    public async Task<ActionResult> DeleteItem([FromRoute] long orderId, [FromRoute] long itemId, CancellationToken ct)
    {
        var command = new DeleteOrderItemCommand(orderId, itemId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPut("{itemId:long}")]
    public async Task<ActionResult<OrderItemModel>> UpdateItem(
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
    public async Task<ActionResult<List<ListOrderItemsResult>>> ListItems([FromRoute] long orderId, CancellationToken ct)
    {
        var query = new ListOrderItemsQuery(orderId);
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }
}