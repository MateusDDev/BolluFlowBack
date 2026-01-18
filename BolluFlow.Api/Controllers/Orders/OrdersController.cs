using BolluFlow.Application.Features.Orders.CancelOrder;
using BolluFlow.Application.Features.Orders.CreateOrder;
using BolluFlow.Application.Features.Orders.DeliverOrder;
using BolluFlow.Application.Features.Orders.FinishOrder;
using BolluFlow.Application.Features.Orders.GetOrderDetails;
using BolluFlow.Application.Features.Orders.ListOrders;
using BolluFlow.Application.Features.Orders.ReopenOrder;
using BolluFlow.Application.Features.Orders.StartOrder;
using BolluFlow.Application.Features.Orders.UpdateOrder;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Orders;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderModel>> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("{orderId:long}/cancel")]
    public async Task<ActionResult> CancelOrder(
        [FromRoute] long orderId, 
        CancellationToken ct)
    {
        var command = new CancelOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPost("{orderId:long}/start")]
    public async Task<ActionResult> StartOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new StartOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPost("{orderId:long}/deliver")]
    public async Task<ActionResult> DeliverOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new DeliverOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPost("{orderId:long}/finish")]
    public async Task<ActionResult> FinishOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new FinishOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPost("{orderId:long}/reopen")]
    public async Task<ActionResult> ReopenOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new ReopenOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPut("{orderId:long}")]
    public async Task<ActionResult> UpdateOrder(
        [FromRoute] long orderId,
        [FromBody] UpdateOrderCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateOrderCommand(orderId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpGet("{orderId:long}")]
    public async Task<ActionResult<OrderModel>> GetOrderDetails([FromRoute] long orderId)
    {
        var query = new GetOrderDetailsQuery(orderId);
        
        var result = await mediator.Send(query);
        
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<OrderModel>>> ListOrders(CancellationToken ct)
    {
        var query = new ListOrdersQuery();
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }
}