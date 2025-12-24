using BolluFlow.Application.Features.Orders.CancelOrder;
using BolluFlow.Application.Features.Orders.CreateOrder;
using BolluFlow.Application.Features.Orders.DeliverOrder;
using BolluFlow.Application.Features.Orders.FinishOrder;
using BolluFlow.Application.Features.Orders.GetOrderDetails;
using BolluFlow.Application.Features.Orders.ListOrders;
using BolluFlow.Application.Features.Orders.ReopenOrder;
using BolluFlow.Application.Features.Orders.StartOrder;
using BolluFlow.Application.Features.Orders.UpdateOrder;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("{orderId:long}/cancel")]
    public async Task<IActionResult> CancelOrder(
        [FromRoute] long orderId, 
        [FromBody] CancelOrderCommandBody body, 
        CancellationToken ct)
    {
        var command = new CancelOrderCommand(orderId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPost("{orderId:long}/start")]
    public async Task<IActionResult> StartOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new StartOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPost("{orderId:long}/deliver")]
    public async Task<IActionResult> DeliverOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new DeliverOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPost("{orderId:long}/finish")]
    public async Task<IActionResult> FinishOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new FinishOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPost("{orderId:long}/reopen")]
    public async Task<IActionResult> ReopenOrder([FromRoute] long orderId, CancellationToken ct)
    {
        var command = new ReopenOrderCommand(orderId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPut("{orderId:long}")]
    public async Task<IActionResult> UpdateOrder(
        [FromRoute] long orderId,
        [FromBody] UpdateOrderCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateOrderCommand(orderId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpGet("{orderId:long}")]
    public async Task<IActionResult> GetOrderDetails([FromRoute] long orderId)
    {
        var query = new GetOrderDetailsQuery(orderId);
        
        var result = await mediator.Send(query);
        
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<IActionResult> ListOrders(CancellationToken ct)
    {
        var query = new ListOrdersQuery();
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }
}